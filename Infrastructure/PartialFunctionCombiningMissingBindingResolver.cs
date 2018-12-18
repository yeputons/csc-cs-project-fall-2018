using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ninject;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using TranspilerInfrastructure;

namespace Infrastructure
{
	public interface PartialFunctionCombined<Tag>
	{ }

	public class PartialFunctionCombiningMissingBindingResolver<Tag> : NinjectComponent, IMissingBindingResolver
	{
		public static void LoadAllTaggedFunctionsFrom(Assembly assembly, IKernel kernel)
		{
			foreach (var type in assembly.GetTypes())
			{
				foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
				{
					if (field.FieldType.IsTaggedFunction<Tag>())
					{
						kernel.Bind(field.FieldType).ToConstant(field.GetValue(null));
					}
				}
				foreach (var iface in type.GetInterfaces())
				{
					if (iface.IsTaggedFunction<Tag>())
					{
						kernel.Bind(iface).To(type);
					}
				}
			}
		}

		public static void AddToKernel(IKernel kernel)
		{
			kernel.Components.Add<IMissingBindingResolver, PartialFunctionCombiningMissingBindingResolver<Tag>>();
		}

		public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, IRequest request)
		{
			Type service = request.Service;
			Type serviceInput, serviceOutput;
			if (!service.IsTaggedFunction<PartialFunctionCombined<Tag>>(out serviceInput, out serviceOutput)) return Enumerable.Empty<IBinding>();

			var partialFunctions = new List<IBinding>();
			foreach (var key in bindings.Keys)
			{
				Type curInput, curOutput;
				if (!key.IsTaggedFunction<Tag>(out curInput, out curOutput)) continue;
				if (!serviceOutput.IsAssignableFrom(curOutput)) continue;
				if (curInput.IsAssignableFrom(serviceInput) || serviceInput.IsAssignableFrom(curInput))
				{
					partialFunctions.AddRange(bindings[key]);
				}
			}

			if (partialFunctions.Count == 0)
			{
				throw new ArgumentException($"No partial functions found for partial functions tagged {typeof(Tag)} from {serviceInput} to {serviceOutput}");
			}

			return new IBinding[]
			{
				new Binding(service)
				{
					Target = BindingTarget.Provider,
					ScopeCallback = StandardScopeCallbacks.Singleton,
					ProviderCallback = context =>
						new PartialFunctionCombiningProvider(service, partialFunctions.Select(binding =>
						{
							Type bindingInput, bindingOutput;
							if (!binding.Service.IsTaggedFunction<Tag>(out bindingInput, out bindingOutput))
								throw new ArgumentException($"Expected TaggedFunction<{typeof(Tag)}, T1, TResult> as a partial function");
							// TODO: probably calling binding.ProviderCallback() is not the best idea because it ignores Ninject's planning.
							// May result in weird behavior if there are unresolved dependencies (e.g. creating object of a wrong type).
							return new PartialFunctionProvider(bindingInput, bindingOutput, binding.ProviderCallback(context));
						}).ToList())
				}
			};
		}

		private struct PartialFunctionProvider
		{
			public Type InputType { get; }
			public Type OutputType { get; }
			public IProvider Provider { get; }

			public PartialFunctionProvider(Type inputType, Type outputType, IProvider provider)
			{
				InputType = inputType;
				OutputType = outputType;
				Provider = provider;
			}
		}

		private struct PartialFunction
		{
			public Type InputType { get; }
			public Type OutputType { get; }
			public Delegate Func { get; }

			public PartialFunction(Type inputType, Type outputType, Delegate func)
			{
				InputType = inputType;
				OutputType = outputType;
				Func = func;
			}
		}

		private class PartialFunctionCombiningProvider : IProvider
		{
			private readonly Type input, output;
			private readonly IReadOnlyList<PartialFunctionProvider> partialFunctionsProviders;

			public PartialFunctionCombiningProvider(Type service, IReadOnlyList<PartialFunctionProvider> partialFunctionsProviders)

			{
				Type = service;
				if (!service.IsTaggedFunction<PartialFunctionCombined<Tag>>(out input, out output))
					throw new ArgumentException($"Expected TaggedFunction<{typeof(PartialFunctionCombined<Tag>)}, T1, TResult> as a service type");
				this.partialFunctionsProviders = partialFunctionsProviders;
			}

			public object Create(IContext context)
			{
				IReadOnlyList<PartialFunction> functions = partialFunctionsProviders.Select(provider =>
				{
					// TODO: probably calling Provider.Create() is not the best idea because it ignores Ninject's planning.
					// May result in weird behavior if there are unresolved dependencies (e.g. creating object of a wrong type).
					// Here are some hacks to make it work.
					IPlan oldPlan = context.Plan;
					context.Plan = null;
					var result = new PartialFunction(provider.InputType, provider.OutputType,
						TaggedFunctionToDelegate(provider.Provider.Create(context)));
					context.Plan = oldPlan;
					return result;
				}).ToList();
				Func<object, object> resultFunc = o =>
				{
					foreach (var func in functions)
					{
						if (func.InputType.IsInstanceOfType(o))
						{
							var result = func.Func.DynamicInvoke(o);
							if (result != null) return result;
						}
					}

					return null;
				};
				Type funcType = typeof(Func<,>).MakeGenericType(input, output);
				Type resultType = typeof(TaggedFuncWrapper<,,>).MakeGenericType(typeof(PartialFunctionCombined<Tag>), input, output);

				var param = Expression.Parameter(typeof(object));
				var funcWithCast = Expression.Lambda(
					Expression.ConvertChecked(
						Expression.Call(Expression.Constant(resultFunc), resultFunc.GetType().GetMethod("Invoke"), param),
						output), param).Compile();
				return resultType.GetConstructor(new[] { funcType }).Invoke(new object[] { funcWithCast });
			}

			private Delegate TaggedFunctionToDelegate(object taggedFunction)
			{
				var taggedFunctionInterface = taggedFunction.GetType().GetInterfaces().First(iface => iface.IsTaggedFunction<Tag>());
				return (Delegate)taggedFunctionInterface.GetProperty("Apply").GetValue(taggedFunction);
			}

			public Type Type { get; }
		}

	}
}

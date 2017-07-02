using System;
using System.Collections.Generic;
using System.Linq;

namespace InversionOfControlContainer
{
    public class Container
    {
        Dictionary<Type, Type> _map;

        public Container()
        {
            _map = new Dictionary<Type, Type>();
        }

        public ContainerBuilder For<TSource>()
        {
            return this.For(typeof(TSource));
        }

		public ContainerBuilder For(Type sourceType)
		{
            return new ContainerBuilder(this, sourceType);
		}



        public object Resolve<TSource>()
        {
            //return (TSource)this.Resolve(typeof(TSource));
			return this.Resolve(typeof(TSource));


		}

		public object Resolve(Type sourceType)
        {
            var destinationType = default(Type);
            _map.TryGetValue(sourceType, out destinationType);
            if (destinationType == null)
            {
                if (sourceType.IsGenericType && 
                    _map.ContainsKey(sourceType.GetGenericTypeDefinition()))
                {
                    var destination = _map[sourceType.GetGenericTypeDefinition()];
                    var closedDestination = destination.MakeGenericType(sourceType.GenericTypeArguments);
                    return CreateInstance(closedDestination);

                }
                if (!sourceType.IsAbstract) 
                {
                    return CreateInstance(sourceType);
                }
                throw new InvalidOperationException("Could not resolve type: " + sourceType);
            }


            return CreateInstance(destinationType);

        }

        private object CreateInstance(Type destinationType)
        {
            var parameters = destinationType.GetConstructors().OrderByDescending(x => x.GetParameters().Count())
                                             .First().GetParameters().Select(p => Resolve(p.ParameterType)).ToArray();
            return Activator.CreateInstance(destinationType, parameters);
        }

        public class ContainerBuilder 
        {
            public ContainerBuilder(Container container, Type sourceType)
            {
                _container = container;
                _sourceType = sourceType;
            }

            Container _container;
            Type _sourceType;

			public ContainerBuilder Use<T>()
			{
                return this.Use(typeof(T));
			}

			public ContainerBuilder Use(Type destinationType)
			{
				_container._map.Add(_sourceType, destinationType);
                return this;
			}

        }
    }
}

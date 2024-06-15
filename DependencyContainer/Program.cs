namespace DependencyContainer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var container = new DependencyContainer();
            container.AddDependencies<HelloService>();
            container.AddDependencies<ServiceConsumer>();
            container.AddDependencies<MessageService>();

            var resolver = new DependencyResolver(container);
            var service = resolver.GetService<HelloService>();
            var consumer = resolver.GetService<ServiceConsumer>();

            service.Print();
            consumer.Print();
        }
    }

    class DependencyResolver
    {
        readonly DependencyContainer _container;
        public DependencyResolver(DependencyContainer container)
        {
            _container = container;
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public object GetService(Type type)
        {
            var serviceType = _container.GetDependency(type);
            var ctorParams = serviceType.GetConstructors()[0].GetParameters();

            if (ctorParams.Length == 0)
            {
                return Activator.CreateInstance(serviceType);
            }

            var paramImplementation = new object[ctorParams.Length];

            for (int i = 0; i < ctorParams.Length; i++)
            {
                paramImplementation[i] = GetService(ctorParams[i].ParameterType);
            }

            return Activator.CreateInstance(type, paramImplementation);
        }
    }

    class DependencyContainer
    {
        List<Type> _dependencies = new List<Type>();

        public void AddDependencies<T>()
        {
            _dependencies.Add(typeof(T));
        }

        public Type GetDependency(Type type)
        {
            return _dependencies.First(x => x.Name == type.Name);
        }
    }

    class ServiceConsumer
    {
        readonly HelloService helloService;

        public ServiceConsumer(HelloService helloService)
        {
            this.helloService = helloService;
        }

        public void Print()
        {
            this.helloService.Print();
        }
    }

    public class HelloService
    {
        MessageService _messageService;

        public HelloService(MessageService messageService)
        {
            this._messageService = messageService;
        }
        public void Print()
        {
            _messageService.Print();
        }
    }

    public class MessageService
    {
        public void Print()
        {
            Console.WriteLine("Hello, World! Message service");
        }
    }
}

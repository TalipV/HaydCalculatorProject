namespace HaydCalculator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = new Window(new AppShell())
            {
                Width = 425,
                Height = 800
            };

            return window;
        }
    }
}
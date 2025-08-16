using WarehouseApp.MAUI.Services;

namespace WarehouseApp.MAUI;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());

		// Initialize database on app startup
		_ = Task.Run(async () =>
		{
			try
			{
				var serviceProvider = Handler?.MauiContext?.Services;
				if (serviceProvider != null)
				{
					var dbInitService = serviceProvider.GetService<IDatabaseInitializationService>();
					if (dbInitService != null)
					{
						await dbInitService.InitializeAsync();
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
			}
		});

		return window;
	}
}
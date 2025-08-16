using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ZXing.Net.Maui;
using WarehouseApp.Core.Interfaces;
using WarehouseApp.Core.Services;
using WarehouseApp.Data.Context;
using WarehouseApp.Data.Interfaces;
using WarehouseApp.Data.Repositories;
using WarehouseApp.MAUI.Services;
using WarehouseApp.MAUI.ViewModels;
using WarehouseApp.MAUI.Views;

namespace WarehouseApp.MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			// .UseBarcodeReader() // Disabled for now - will be configured later
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Configure Services
		ConfigureServices(builder.Services);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		// Initialize ServiceHelper
		ServiceHelper.Initialize(app.Services);

		return app;
	}

	private static void ConfigureServices(IServiceCollection services)
	{
		// Database Configuration
		ConfigureDatabase(services);

		// Core Services
		ConfigureCoreServices(services);

		// MAUI Services
		ConfigureMauiServices(services);

		// ViewModels
		ConfigureViewModels(services);

		// Views
		ConfigureViews(services);
	}

	private static void ConfigureDatabase(IServiceCollection services)
	{
		// SQLite Database Configuration
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "warehouse.db");

		services.AddDbContext<WarehouseDbContext>(options =>
		{
			options.UseSqlite($"Data Source={dbPath}");

#if DEBUG
			options.EnableSensitiveDataLogging();
			options.EnableDetailedErrors();
#endif
		});

		// Repository Pattern
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
	}

	private static void ConfigureCoreServices(IServiceCollection services)
	{
		// Business Logic Services
		services.AddScoped<IProductService, ProductService>();
		services.AddScoped<IStockService, StockService>();

		// Add other services when implemented
		// services.AddScoped<ICategoryService, CategoryService>();
		// services.AddScoped<ILocationService, LocationService>();
		// services.AddScoped<IUserService, UserService>();
		// services.AddScoped<ISupplierService, SupplierService>();
	}

	private static void ConfigureMauiServices(IServiceCollection services)
	{
		// Navigation Service
		services.AddSingleton<INavigationService, NavigationService>();

		// Database Initialization Service
		services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();

		// Photo Service
		services.AddSingleton<IPhotoService, PhotoService>();

		// Barcode Service
		services.AddSingleton<IBarcodeService, BarcodeService>();

		// Search Service
		services.AddScoped<ISearchService, SearchService>();

		// Real-time Service
		services.AddSingleton<IRealtimeService, RealtimeService>();

		// Platform Services (when implemented)
		// services.AddSingleton<IConnectivityService, ConnectivityService>();
		// services.AddSingleton<IDialogService, DialogService>();
		// services.AddSingleton<IFileService, FileService>();
		// services.AddSingleton<IPreferencesService, PreferencesService>();
		// services.AddSingleton<IBarcodeService, BarcodeService>();
	}

	private static void ConfigureViewModels(IServiceCollection services)
	{
		// ViewModels with Transient lifetime for proper navigation
		services.AddTransient<MainViewModel>();
		services.AddTransient<ProductListViewModel>();
		services.AddTransient<PhotoCaptureViewModel>();
		services.AddTransient<BarcodeScannerViewModel>();
		services.AddTransient<AdvancedSearchViewModel>();
		services.AddTransient<RealtimeViewModel>();

		// Add other ViewModels when implemented
		// services.AddTransient<ProductDetailViewModel>();
		// services.AddTransient<AddEditProductViewModel>();
		// services.AddTransient<CategoryListViewModel>();
		// services.AddTransient<LocationListViewModel>();
		// services.AddTransient<StockMovementViewModel>();
		// services.AddTransient<ReportsViewModel>();
		// services.AddTransient<SettingsViewModel>();
	}

	private static void ConfigureViews(IServiceCollection services)
	{
		// Views with Transient lifetime
		services.AddTransient<DashboardPage>();
		services.AddTransient<ProductListPage>();
		services.AddTransient<PhotoCapturePage>();
		services.AddTransient<BarcodeScannerPage>();

		// Add other Views when implemented
		// services.AddTransient<ProductDetailPage>();
		// services.AddTransient<AddEditProductPage>();
		// services.AddTransient<CategoryListPage>();
		// services.AddTransient<LocationListPage>();
		// services.AddTransient<StockMovementPage>();
		// services.AddTransient<ReportsPage>();
		// services.AddTransient<SettingsPage>();
	}
}

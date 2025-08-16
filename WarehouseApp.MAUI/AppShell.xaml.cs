using WarehouseApp.MAUI.Views;

namespace WarehouseApp.MAUI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		RegisterRoutes();
	}

	private static void RegisterRoutes()
	{
		// Register additional routes for navigation
		// Main routes are already defined in XAML

		// Product routes
		Routing.RegisterRoute("products/add", typeof(ProductListPage)); // Will be AddEditProductPage
		Routing.RegisterRoute("products/edit", typeof(ProductListPage)); // Will be AddEditProductPage
		Routing.RegisterRoute("products/details", typeof(ProductListPage)); // Will be ProductDetailPage

		// Stock routes
		Routing.RegisterRoute("stock/in", typeof(DashboardPage)); // Will be StockInPage
		Routing.RegisterRoute("stock/out", typeof(DashboardPage)); // Will be StockOutPage
		Routing.RegisterRoute("stock/transfer", typeof(DashboardPage)); // Will be TransferPage

		// Other routes
		Routing.RegisterRoute("scanner", typeof(BarcodeScannerPage));
		Routing.RegisterRoute("barcode-scanner", typeof(BarcodeScannerPage));
		Routing.RegisterRoute("categories", typeof(DashboardPage)); // Will be CategoryListPage
		Routing.RegisterRoute("locations", typeof(DashboardPage)); // Will be LocationListPage
		Routing.RegisterRoute("photo-capture", typeof(PhotoCapturePage));

		// Modal routes
		Routing.RegisterRoute("modal/product/add", typeof(ProductListPage)); // Will be AddEditProductPage
		Routing.RegisterRoute("modal/product/edit", typeof(ProductListPage)); // Will be AddEditProductPage
	}
}

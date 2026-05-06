using BlogApp.Data.Concrete.EFCore;
using EFCoreApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext'in service registration tanýmlamasý burada yapýlýyor.
builder.Services.AddDbContext<DataContext>(options =>
{
    var config = builder.Configuration; // Burada; WepApplicationBuilder class'ýnýn ConfigrationManager type'ýnda olan Configration property'sini çaðýrýyoruz. Bu bize ConfigrationManager type'ýnda bir instance veriyor. Bu instance, appsettings.json dosyasýna eriþebilmemizi saðlýyor. 
    var connectionStrings = config.GetConnectionString("Database"); // Daha sonra da burada config instance'ý üzerinden GetConnectionString() metodunu çaðýrýyoruz. Ve bu metoda oluþturduðumuz baðlantýnýn ismini veriyoruz. Bu bize eriþtiðimiz appsettings.json dosyasýndan, parametre olarak verdiðimiz, database baðlantý yolunun value'sini, yani içeriðini veriyor.  
    options.UseSqlite(connectionStrings); // Burada da aldýðýmýz baðlantýyý Sqlite'ýn provider'ý üzerine gönderip, bu projenin buluduðu dosyaya bir server oluþturmuþ oluyoruz.
});
// Burasýna mssql sqlserver ile baðlantý registration'u.
/*builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});*/


var app = builder.Build();

SeedData.TestVerileriniDoldur(app); // Burada IoC Container harcinde, ayrý bir statik class ve metot içerisinde, service provider'a eriþen ve istediði class'ýn instance'ýný türeten bir class'ýn ve metodunu çaðýrmýþ olduk. // Data klasöründeki SeedData.cs dosyasýna git.

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

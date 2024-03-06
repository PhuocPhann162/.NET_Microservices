using FucoMicro.Web.Service;
using FucoMicro.Web.Service.IService;
using FucoMicro.Web.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Config for HttpClientFactory
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Config CouponService will use HttpClient
builder.Services.AddHttpClient<ICouponService, CouponService>();

// Config for using coupon api url 
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];


// Config for Scoped Lifetime in DI
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using RockPaperScissors.DB;
using RockPaperScissors.Server;

namespace RockPaperScissors
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetupASP(args);

            ServerEmulator.Database.CreateChangeRequest("users", new FlexibleDB.Value("logged_in_device", ""));
           
            Console.ReadLine();
        }

        public static async void SetupASP(string[] args)
        {
            await Task.Run(() =>
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddDistributedMemoryCache();

                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromSeconds(100);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });

                // Add services to the container.
                builder.Services.AddControllersWithViews();

                var app = builder.Build();

                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();
                app.UseSession();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            });
        }
    }
}

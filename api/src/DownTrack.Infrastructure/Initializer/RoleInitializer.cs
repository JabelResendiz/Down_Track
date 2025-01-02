
using DownTrack.Domain.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DownTrack.Infrastructure.Initializer;

public class RoleInitializer : IHostedService
{
    // using 
    private readonly IServiceProvider _serviceProvider;

    public RoleInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    /// <summary>
    /// This method is called when the application starts. It allows the background service to perform initialization tasks.
    /// The provided cancellationToken can be used to stop or cancel the service if necessary.
    /// </summary>
    /// <param name="cancellationToken">Allows the service to be stopped or cancelled if necessary. It can be used to handle graceful shutdowns or cancellation requests.</param>
    /// <returns>Returns a Task that represents the asynchronous operation. The Task completes when the service has finished its start-up process.</returns>
    /// <exception cref="Exception">Thrown if there is an error during the initialization or starting of the service. This can be used to signal failure in the service startup process.</exception>


    public async Task StartAsync(CancellationToken cancellationToken)
    {

        using (var scope = _serviceProvider.CreateScope())
        {
            //get RoleManager<IdentityRole> service from the dependency container
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in UserRoleHelper.AllRoles())
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Error create the role: {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
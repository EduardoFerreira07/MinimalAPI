using System.Collections.Generic;
using System.Linq;
using MinimalAPI.Domain.Entities;

namespace MinimalAPI.Domain.ModelViews;

public static class AdministratorModelViewFactory
{
    public static AdministratorModelView FromEntity(Administrator admin)
        => new AdministratorModelView(admin.Id, admin.Email, admin.Perfil);

    public static IEnumerable<AdministratorModelView> FromEntityList(IEnumerable<Administrator> admins)
        => admins.Select(FromEntity);
}

using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class AppRole
    {
        public string Id { get; set; }
        public string Name { get; set; }


        public AppRole ToModel(IdentityRole entity)
        {
            if (entity == null) return null;
            return new AppRole
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public List<AppRole> ToModel(List<IdentityRole> entities)
        {
            var model = new List<AppRole>();
            if (entities != null) model.AddRange(entities.Select(s => ToModel(s)));
            return model;
        }
    }
}

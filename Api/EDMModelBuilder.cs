using BusinessObject;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace FOMSOData
{
    public class EDMModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<User>("Users");

            return modelBuilder.GetEdmModel();
        }
    }
}

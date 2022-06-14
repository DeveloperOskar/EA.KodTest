using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace EA.KodTest.Api.Packages.Features.GetPackageMeasures
{
    using Response = DataContracts.Output.Response;
    using DataModel = DataModels.PackageMeasures;
    public class GetPackageMeasures
    {
        [FunctionName("GetPackageMeasures")]
        public async Task<IActionResult> GET(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "packages/{packagenumber}/measures")] HttpRequest req, string packagenumber)
        {
            var isNumeric = long.TryParse(packagenumber, out long _);

            if (packagenumber.Length != 18 || isNumeric == false)
                return new BadRequestObjectResult($"Invalid packagenumber: {packagenumber}");

            var response = new Response();

            using (StreamReader r = new StreamReader("../../../mockData.json"))
            {
                string mockDb = await r.ReadToEndAsync();
                var packages = JsonSerializer.Deserialize<Dictionary<string, DataModel>>(mockDb);

                if (packages.ContainsKey(packagenumber) == false)
                    return new NotFoundObjectResult($"Package with packagnumber: {packagenumber} was not found.");

               /*
                 I did not understand this part of the test: 

                "Idag har vi en begränsning på hur stora paket får vara som går igenom våra scanningsmaskiner. 
                 Följande mått får ej överskridas". 
                
                This check should really be done when you save a package.
                So there should be no way a package can have exceed the limits. So I just implemented a small check here.
               */

                if (packages[packagenumber].ValidatePackage() == false)
                    return new UnprocessableEntityObjectResult($"Package with packagenumber: {packagenumber} has invalid measures");

                MapResponse(response, packages[packagenumber]);
            };

            return new OkObjectResult(response);
        }

        private void MapResponse(Response response, DataModel packageMeasures)
        {
            response.Weight = packageMeasures.Weight;
            response.Height = packageMeasures.Height;
            response.Width = packageMeasures.Width;
            response.Length = packageMeasures.Length;
        }
    }
}

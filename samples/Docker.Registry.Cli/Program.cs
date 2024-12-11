using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Domain.Catalogs;
using Docker.Registry.DotNet.Domain.Registry;

namespace Docker.Registry.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TestAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
               
            }
        }

        static async Task TestAsync()
        {
            //http://myi.dockerregistry:30000/v2/_catalog
            string url = "http://myi.dockerregistry:30000";

            var configuration = new RegistryClientConfiguration(url);

            using (var client = configuration.CreateClient())
            {

               

                var catalog = await client.Catalog.GetCatalog(new CatalogParameters()
                {
                    Number = 10
                });

                var repositories = catalog.Repositories;

                foreach (var repository in repositories)
                {
                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine(repository);
                    Console.WriteLine("-----------------------------------------------");

                    var tags = await client.Tags.ListImageTagsAsync(repository, new ListImageTagsParameters());

                    foreach (var tag in tags.Tags)
                    {
                        Console.WriteLine($"  {tag}");
                    }
                }


                //var tags = await client.Tags.ListImageTagsAsync("resin", new ListImageTagsParameters());

                //foreach (var tag in tags.Tags)
                //{
                //    Console.WriteLine();
                //}



                //if (string.IsNullOrWhiteSpace(repository))
                //{
                //    Console.WriteLine("No repository found.");
                //}
                //else
                //{
                //    var tagsReponse =  await client.Tags.ListImageTagsAsync(repository, new ListImageTagsParameters());

                //    var tag = tagsReponse.Tags.FirstOrDefault();

                //    if (string.IsNullOrWhiteSpace(tag))
                //    {
                //        Console.WriteLine("No tags found.");
                //    }
                //    else
                //    {
                //        var manifestResult = await client.Manifest.GetManifestAsync(repository, tag);

                //        Console.WriteLine(manifestResult.Manifest.GetType().Name);

                //        var imageManifest = manifestResult.Manifest as ImageManifest2_1;

                //        if (imageManifest != null)
                //        {
                //            var layer = imageManifest.FsLayers.First();

                //            var getBlobResponse = await client.Blobs.GetBlobAsync(repository, layer.BlobSum);

                //            Console.WriteLine($"\t\tDigetst: {getBlobResponse.DockerContentDigest}");

                //            using (getBlobResponse.Stream)
                //            using (Stream targetStream = File.OpenWrite(@"c:\test.layer"))
                //            {
                //                await getBlobResponse.Stream.CopyToAsync(targetStream);
                //            }   
                //        }
                //    }

                //}

            }
        }
    }
}

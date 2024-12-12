
using Docker.Registry.DotNet.Domain.Catalogs;
using Docker.Registry.DotNet.Domain.ImageReferences;
using Docker.Registry.DotNet.Domain.Models;
using Docker.Registry.DotNet.Domain.Registry;
using NUnit.Framework;

namespace Docker.Registry.DotNet.Tests;

[TestFixture]
public class ClientTests
{

    /*
     
    成功
    curl -X GET   -H "Accept: application/vnd.docker.distribution.manifest.v2+json, application/vnd.oci.image.index.v1+json"   "http://myi.dockerregistry:30000/v2/k8sapi/manifests/latest"

     */

    static IRegistryClient client { get; set; }
    static string LogPath { get; set; }
    static ClientTests()
    {

        //http://myi.dockerregistry:30000/v2/_catalog
        //
        string url = "http://myi.dockerregistry:30000";
        var configuration = new RegistryClientConfiguration(url);
        client = configuration.CreateClient();

        LogPath = Path.Combine(Environment.CurrentDirectory.Replace("\\bin\\Debug\\net8.0",string.Empty), "log.txt");
        GetCatalog();
    }

    static IReadOnlyCollection<string> Repositories { get; set; }
    public static  IReadOnlyCollection<string> GetCatalog()
    {

        if (Repositories != null)
            return Repositories;

        File.AppendAllText(LogPath, $"{Environment.NewLine}============");
        File.AppendAllText(LogPath, $"{Environment.NewLine}GetCatalog begin{Environment.NewLine}");

        CatalogResponse catalog =  client.Catalog.GetCatalog(new CatalogParameters()
        {
            Number = 10
        }).Result;

        Repositories = catalog.Repositories;
        foreach (var repository in Repositories)
        {
            File.AppendAllText(LogPath, $"{Environment.NewLine}repository={repository}");
        }

        File.AppendAllText(LogPath, $"{Environment.NewLine}{Environment.NewLine}GetCatalog end{Environment.NewLine}");


        return Repositories;

    }

    [Test]
    public void ListTags()
    {
        try
        {
            File.AppendAllText(LogPath, $"{Environment.NewLine}============");
            File.AppendAllText(LogPath, $"{Environment.NewLine}ListTags begin{Environment.NewLine}");

            foreach (var repository in  GetCatalog())
            {

                File.AppendAllText(LogPath, $"{Environment.NewLine}repository={repository}");

                ListTagResponseModel tags =  client.Tags.ListTags(repository, new ListTagsParameters() { Number = 100 }).Result;
                File.AppendAllText(LogPath, $"{Environment.NewLine}tags.Name={tags.Name},");

                foreach (ImageTag imageTag in tags.Tags)
                {
                    File.AppendAllText(LogPath, $"{Environment.NewLine}imageTag.Value={imageTag.Value},");
                }
                File.AppendAllText(LogPath, Environment.NewLine);
            }

            File.AppendAllText(LogPath, $"{Environment.NewLine}{Environment.NewLine}ListTags end{Environment.NewLine}");

        }
        catch (Exception ee)
        {
            var t = ee;
        }

    }


    [Test]
    public void ListTagsByDigests()
    {
        try
        {
            File.AppendAllText(LogPath, $"{Environment.NewLine}============");
            File.AppendAllText(LogPath, $"{Environment.NewLine}ListTagsByDigests begin{Environment.NewLine}");

            foreach (var repository in GetCatalog())
            {

                File.AppendAllText(LogPath, $"{Environment.NewLine}repository={repository}");

                ListTagByDigestResponseModel tags = client.Tags.ListTagsByDigests(repository).Result;
                File.AppendAllText(LogPath, $"{Environment.NewLine}tags.Name={tags.Name},");

                foreach (DigestTagModel digestTag in tags.Tags)
                {
                    File.AppendAllText(LogPath, $"{Environment.NewLine}digestTag.Digest.Value={digestTag.Digest.Value},");

                    foreach (ImageTag imageTag in digestTag.Tags) 
                    {
                        File.AppendAllText(LogPath, $"{Environment.NewLine}imageTag.Value={imageTag.Value},");
                    }

                }
                File.AppendAllText(LogPath, Environment.NewLine);
            }

            File.AppendAllText(LogPath, $"{Environment.NewLine}{Environment.NewLine}ListTagsByDigests end{Environment.NewLine}");

        }
        catch (Exception ee)
        {
            var t = ee;
        }

    }

}
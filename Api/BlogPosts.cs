using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Models;
using System.Configuration;

namespace Api;
public static class BlogPosts
{
    public static object UriFactory { get; private set; }
    
    [FunctionName($"{nameof(BlogPosts)}_Get")]
    public static IActionResult GetAllBlogPosts(
        [HttpTrigger(AuthorizationLevel.Anonymous,"get", Route = "blogposts")] HttpRequest req,
        [CosmosDB(
        "SwaBlog", 
        "BlogContainer",
        Connection ="CosmosDbConnectionString",
        SqlQuery= @"
        SELECT
        c.id,
        c.Title,
        c.Author,
        c.PublishedDate,
        LEFT(c.BlogPostMarkdown, 500)
        As BlogPostMarkdown,
        Length(c.BlogPostMarkdown) <= 500
        As PreviewIsComplete,
        c.Tags
        FROM c
        WHERE c.Status = 2"
       )] IEnumerable<BlogPost> blogPosts,
        ILogger log)
    {
        return new OkObjectResult(blogPosts);
    }
}
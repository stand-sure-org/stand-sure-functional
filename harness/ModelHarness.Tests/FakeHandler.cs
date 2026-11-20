namespace ModelHarness.Tests;

using System.Net;
using System.Net.Http;
using System.Text;

internal sealed class FakeHandler(HttpStatusCode status, string body) : HttpMessageHandler
{
    public HttpRequestMessage? Request { get; private set; }

    public string? RequestBody { get; private set; }

    public int Calls { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        this.Calls++;
        this.Request = request;
        this.RequestBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        return new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
    }
}

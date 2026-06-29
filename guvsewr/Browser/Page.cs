public class Page
{
    public Uri url;
    public string html;

    public int statusCode;

    public Page(Uri url,string html,int statusCode)
    {
        this.url = url;
        this.html = html;
        this.statusCode = statusCode;
    }
}
using System.Collections.Generic;

namespace Estudos_API.Hateoas
{
    public class Hateoas
    {
        private string Url { get; set; }
        private string Protocol = "https://";
        public List<Link> Actions = new List<Link>();
        public Hateoas(string url)
        {
            Url = url;
        }
        public Hateoas(string url, string protocol)
        {
            Url = url;
            Protocol = protocol;
        }
        public void AddAction(string rel, string method)
        {
            // https://localhost:5200/api/v1/Produtos
            Actions.Add(new Link(Protocol + Url, rel, method));
        }
        public Link[] GetActions(string sufix) // asp net traduz um array em json muito mais fácil
        {
            Link[] tempLinks = new Link[Actions.Count];

            for(int i = 0; i < tempLinks.Length; i++)
            {
                tempLinks[i] = new Link(Actions[i].Href, Actions[i].Rel, Actions[i].Method);
            }
            foreach(var link in tempLinks)
            {
                // https:// localhost:5201/api/v1/Produtos/ 2/32/thais
                link.Href = link.Href + "/" + sufix;
            }
            return tempLinks;
        }
    }
}
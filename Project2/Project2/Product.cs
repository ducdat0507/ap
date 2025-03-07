namespace Project2 
{
    public class Product 
    {
        public string ID;
        public string Name;
        public decimal Price;

        public Product(string id, string name, decimal price)
        {
            ID = id;
            Name = name;
            Price = price;
        }
    }
}
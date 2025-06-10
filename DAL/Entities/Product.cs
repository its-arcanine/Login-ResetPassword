namespace DAL.Entities
{
    public class Product
    {
        // Primary Key - EF Core will recognize ProductId by convention
        public string ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        // Decimal properties should have precision configured in OnModelCreating
        public decimal ProductPrice { get; set; }

        public int ProductQuantity { get; set; }

        public string ProductImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        // Foreign Key to the Category entity
        public string CategoryId { get; set; }

        // Navigation property for the Category this product belongs to
     //   public Category Category { get; set; } // Assuming you have a Category entity
    }
}
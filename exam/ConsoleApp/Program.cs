
using Domain.Models;
using Infrastructure.Repositories;


var s = new ProductRepository();
var d = new CategoryRepository();

while (true)
{
    System.Console.WriteLine("================================================================================");
        System.Console.WriteLine("1 Get All Product");
    System.Console.WriteLine("=============================================================================");
        System.Console.WriteLine("2 Get Product by ID");
    System.Console.WriteLine("=============================================================================");
        System.Console.WriteLine("3 Get All Categories");
    System.Console.WriteLine("=============================================================================");
        System.Console.WriteLine("4 Get Category by ID");
    System.Console.WriteLine("=============================================================================");
        System.Console.WriteLine("5 Add Category");
    System.Console.WriteLine("=============================================================================");
        System.Console.WriteLine("6 Add Product");
     int a = Convert.ToInt32(Console.ReadLine());
    switch (a)
    {
        case 1:
            var products = s.GetAllProducts();
            foreach (var product in products)
            {
                Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}, Quantity: {product.Quantity}, CategoryId: {product.CategoryId}");
                Console.WriteLine();
            }
            break;
        case 2:
            Console.Write("Enter product ID: ");
            int productId = Convert.ToInt32(Console.ReadLine());
            var productById = s.GetProductById(productId);
            if (productById != null)
            {
                Console.WriteLine($"Id: {productById.Id}, Name: {productById.Name}, Price: {productById.Price}, Quantity: {productById.Quantity}, CategoryId: {productById.CategoryId}");
            }
            else
            {
                Console.WriteLine("Product not found.");
            }
            break;
        case 3:
            var categories = d.GetAllCategories();
            foreach (var category in categories)
            {
                Console.WriteLine($"Id: {category.Id}, Name: {category.Name}");
            }
            break;
        case 4:
            Console.Write("Enter category ID: ");
            int categoryId = Convert.ToInt32(Console.ReadLine());
            var categoryById = d.GetCategoryById(categoryId);
            if (categoryById != null)
            {
                Console.WriteLine($"Id: {categoryById.Id}, Name: {categoryById.Name}");
            }
            else
            {
                Console.WriteLine("Category not found.");
            }
            break;
        case 5:
            Console.Write("Enter category name: ");
            string categoryName = Console.ReadLine();
            var newCategory = new Category { Name = categoryName };
            d.AddCategory(newCategory);
            Console.WriteLine("Category added.");
            break;
        case 6:
            Console.Write("Enter product name: ");
            string productName = Console.ReadLine();
            Console.Write("Enter product price: ");
            decimal productPrice = Convert.ToDecimal(Console.ReadLine());
            Console.Write("Enter product quantity: ");
            int productQuantity = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter category ID: ");
            int productCategoryId = Convert.ToInt32(Console.ReadLine());
            var newProduct = new Product { Name = productName, Price = productPrice, Quantity = productQuantity, CategoryId = productCategoryId };
            s.AddProduct(newProduct);
            Console.WriteLine("Product added.");
            break;
}
}
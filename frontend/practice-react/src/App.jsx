import { useEffect, useState } from "react";

function App() {
  const [products, setProducts] = useState([]);
  const [search, setSearch] = useState("");

  useEffect(() => {
    loadProducts();
  }, []);

  async function loadProducts() {
    const response = await fetch("http://localhost:5000/api/products");
    if (!response.ok) return;

    const data = await response.json();
    setProducts(data);
  }

  async function searchProducts() {
    const response = await fetch(
      `http://localhost:5000/api/products/search?term=${encodeURIComponent(search)}`
    );
    if (!response.ok) return;

    const data = await response.json();
    setProducts(data);
  }

  return (
    <div>
      <h1>Products</h1>

      <input
        value={search}
        onChange={e => setSearch(e.target.value)}
        placeholder="Search product"
      />

      <button onClick={searchProducts}>Search</button>
      <button onClick={loadProducts}>Show All</button>

      <ul>
        {products.map(product => (
          <li key={product.id}>
            {product.name} - ${product.price} - {product.category}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;

# Junior LINQ Practice

This project is intentionally simple.

The .NET API uses an in-memory list and focuses on basic LINQ only:

- Where
- OrderBy
- OrderByDescending
- FirstOrDefault
- ToList
- Contains

## Practice questions

Try changing the controller to solve these:

1. Return all products with Price > 100
2. Return only Tech products
3. Sort products from cheapest to most expensive
4. Return the most expensive product
5. Search products by part of the name
6. Return the first Office product
7. Return product names only using Select
8. Count how many Tech products there are
9. Check if any product costs more than 1000 using Any
10. Find the average product price using Average

## Run backend

```bash
cd backend/Practice.Api
dotnet run --urls http://localhost:5000
```

## Run frontend

```bash
cd frontend/practice-react
npm install
npm run dev
```

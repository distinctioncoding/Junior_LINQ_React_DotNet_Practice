# Unit Tests

This project has two sets of unit tests:

| Part | Tool | Location | What it tests |
| --- | --- | --- | --- |
| Backend (.NET) | [xUnit](https://xunit.net/) | `backend/Practice.Api.Tests/` | The LINQ queries in `ProductsController` |
| Frontend (React) | [Vitest](https://vitest.dev/) + [React Testing Library](https://testing-library.com/docs/react-testing-library/intro/) | `frontend/practice-react/src/App.test.jsx` | The `App` component: loading, searching and "Show All" |

## What is a unit test?

A unit test is a small piece of code that runs one small piece of your app
(a "unit", such as one method or one component) and checks that the result is what you expect.

Every test follows the same three steps, often called **Arrange, Act, Assert**:

1. **Arrange**: set up what the test needs (a controller, some fake data).
2. **Act**: call the thing you are testing (`controller.Search("lap")`, click a button).
3. **Assert**: check the result (`Assert.Equal("Laptop", product.Name)`).

## Why do we have them?

- **Fast feedback.** The tests run in a few seconds. You don't need to start the API,
  start the React app, open a browser and click around to see if something still works.
- **Safety net while you practise.** The practice questions in the [README](README.md) ask you
  to change the controller. If one of your changes breaks an existing endpoint, a test turns red
  and tells you exactly which one, instead of you finding out later.
- **The tests describe the expected behaviour.** Reading `ProductsControllerTests.cs`
  is a quick way to learn what each endpoint should return. For example,
  "cheap" means *under 300, sorted from cheapest to most expensive*.
- **Practising testing is part of the job.** Most teams expect developers to write tests with
  their code, so this is a good place to learn how.

## What each test checks

### Backend: `ProductsControllerTests.cs`

The tests create a `ProductsController` and call its methods directly, like a normal C# class.
They don't start a web server or send HTTP requests.

| Endpoint | Method | Tests check that... |
| --- | --- | --- |
| `GET api/products` | `GetAll` | all 5 products come back, in order |
| `GET api/products/search?term=` | `Search` | part of a name matches (`"lap"` finds Laptop), case is ignored (`"LAP"` also works), several matches all come back, no match gives an empty list, and an empty or blank search returns everything |
| `GET api/products/category/{category}` | `GetByCategory` | `Tech` and `Office` return only their own products, and an unknown category returns an empty list |
| `GET api/products/cheap` | `GetCheapProducts` | only products under 300 come back, sorted by price, cheapest first |
| `GET api/products/expensive` | `GetMostExpensive` | the most expensive product (Laptop, 1200) comes back |

### Frontend: `App.test.jsx`

The tests render the `App` component in a fake browser (jsdom) and use it like a user would:
type into the search box and click buttons.

The real API is **not** called. The tests replace `fetch` with a fake version (a *mock*)
that returns fixed data, so the tests can run without the backend running.

| Test | Checks that... |
| --- | --- |
| loads and shows all products when it first renders | the app calls `GET /api/products` on start-up and lists every product |
| searches by the typed term and shows only the results | typing `lap` and clicking **Search** calls the search endpoint and shows only the matching product |
| shows an empty list when the search finds nothing | a search with no results clears the list |
| encodes special characters in the search term | typing `#` sends `term=%23`, so the browser doesn't cut the URL short |
| keeps the current list when the API returns an error | an error response (such as 400) leaves the list as it was instead of crashing the page |
| shows every product again after clicking Show All | **Show All** loads the full list again after a search |

## How to run the tests

### Before you start

You need:

- **.NET 8 SDK** or newer. Check with `dotnet --version`.
- **Node.js 22.13** or newer. Check with `node --version`.

All the commands below are run from the root folder of this repository
(the folder that contains `README.md`).

### Run the backend tests

1. Go to the test project folder:

   ```bash
   cd backend/Practice.Api.Tests
   ```

2. Run the tests. The first run downloads the xUnit packages and builds both projects,
   so it takes longer than later runs.

   ```bash
   dotnet test
   ```

3. Read the summary at the end. You should see something like:

   ```
   Passed!  - Failed: 0, Passed: 15, Skipped: 0, Total: 15
   ```

   If a test fails, the output shows the test name, what it expected and what it actually got.

To see the name of every test as it runs:

```bash
dotnet test --logger "console;verbosity=normal"
```

To run only some tests, filter by name:

```bash
dotnet test --filter "FullyQualifiedName~Search"
```

### Run the frontend tests

1. Go to the React app folder:

   ```bash
   cd frontend/practice-react
   ```

2. Install the packages. You only need to do this the first time, or after
   `package.json` changes.

   ```bash
   npm install
   ```

3. Run the tests:

   ```bash
   npm test
   ```

4. Read the summary at the end. You should see something like:

   ```
   Test Files  1 passed (1)
        Tests  6 passed (6)
   ```

To keep the tests running and re-run them every time you save a file (watch mode):

```bash
npx vitest
```

Press `q` to quit watch mode.

## Adding your own tests

When you solve a practice question from the [README](README.md), add a test for it.
For example, for *"Return all products with Price > 100"*, after adding a `GetOver100`
action to the controller you could add this to `ProductsControllerTests.cs`:

```csharp
[Fact]
public void GetOver100_ReturnsOnlyProductsOver100()
{
    var products = GetOkValue(_controller.GetOver100());

    Assert.Equal(new[] { "Laptop", "Chair", "Desk" }, products.Select(p => p.Name));
    Assert.All(products, p => Assert.True(p.Price > 100));
}
```

Then run `dotnet test` again and check that it passes.

Tips:

- Give each test a name that says what it checks, such as `Method_Situation_ExpectedResult`.
- Test the edge cases as well as the normal case: no results, empty input, different letter case.
- Make a test fail on purpose once (change the expected value) so you know it really checks something.

## Troubleshooting

- **`Filename too long` when cloning on Windows.** Clone with long paths enabled:
  `git -c core.longpaths=true clone <repo-url>`, or clone into a shorter folder such as `C:\code`.
- **`dotnet test` fails with "file is being used by another process" (Windows).** The API is still
  running from `dotnet run` and has locked `Practice.Api.exe`. Stop it (`Ctrl+C` in its terminal),
  then run the tests again.
- **`dotnet` can't find .NET 8.** Install the .NET 8 SDK from https://dotnet.microsoft.com/download.
- **`npm test` errors about the Node version.** Upgrade Node.js to 22.13 or newer.
- **Changed files under `backend/Practice.Api/obj/` after running the tests.** Those are build
  files. Don't commit them. Undo the changes with `git checkout -- backend/Practice.Api/obj`.

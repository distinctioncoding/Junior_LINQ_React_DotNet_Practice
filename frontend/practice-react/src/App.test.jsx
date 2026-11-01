import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import App from "./App";

const API = "http://localhost:5000/api/products";

const allProducts = [
  { id: 1, name: "Laptop", price: 1200, category: "Tech" },
  { id: 2, name: "Mouse", price: 30, category: "Tech" },
  { id: 3, name: "Chair", price: 200, category: "Office" },
];

// Fake the backend: each URL returns the data we give it.
// Any other URL behaves like an API error (400 Bad Request).
function mockApi(responses) {
  vi.stubGlobal(
    "fetch",
    vi.fn(url => {
      const found = url in responses;
      return Promise.resolve({
        ok: found,
        status: found ? 200 : 400,
        json: () => Promise.resolve(found ? responses[url] : { title: "Bad Request" }),
      });
    })
  );
}

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("App", () => {
  it("loads and shows all products when it first renders", async () => {
    mockApi({ [API]: allProducts });

    render(<App />);

    expect(await screen.findByText("Laptop - $1200 - Tech")).toBeInTheDocument();
    expect(screen.getByText("Mouse - $30 - Tech")).toBeInTheDocument();
    expect(screen.getByText("Chair - $200 - Office")).toBeInTheDocument();
    expect(fetch).toHaveBeenCalledWith(API);
  });

  it("searches by the typed term and shows only the results", async () => {
    mockApi({
      [API]: allProducts,
      [`${API}/search?term=lap`]: [allProducts[0]],
    });
    const user = userEvent.setup();
    render(<App />);
    await screen.findByText("Mouse - $30 - Tech");

    await user.type(screen.getByPlaceholderText("Search product"), "lap");
    await user.click(screen.getByRole("button", { name: "Search" }));

    expect(await screen.findAllByRole("listitem")).toHaveLength(1);
    expect(screen.getByText("Laptop - $1200 - Tech")).toBeInTheDocument();
    expect(fetch).toHaveBeenCalledWith(`${API}/search?term=lap`);
  });

  it("shows an empty list when the search finds nothing", async () => {
    mockApi({
      [API]: allProducts,
      [`${API}/search?term=phone`]: [],
    });
    const user = userEvent.setup();
    render(<App />);
    await screen.findByText("Mouse - $30 - Tech");

    await user.type(screen.getByPlaceholderText("Search product"), "phone");
    await user.click(screen.getByRole("button", { name: "Search" }));

    await vi.waitFor(() => expect(screen.queryAllByRole("listitem")).toHaveLength(0));
  });

  it("encodes special characters in the search term", async () => {
    mockApi({
      [API]: allProducts,
      [`${API}/search?term=%23`]: [],
    });
    const user = userEvent.setup();
    render(<App />);
    await screen.findByText("Mouse - $30 - Tech");

    await user.type(screen.getByPlaceholderText("Search product"), "#");
    await user.click(screen.getByRole("button", { name: "Search" }));

    expect(fetch).toHaveBeenCalledWith(`${API}/search?term=%23`);
  });

  it("keeps the current list when the API returns an error", async () => {
    mockApi({ [API]: allProducts });
    const user = userEvent.setup();
    render(<App />);
    await screen.findByText("Mouse - $30 - Tech");

    await user.type(screen.getByPlaceholderText("Search product"), "lap");
    await user.click(screen.getByRole("button", { name: "Search" }));

    await vi.waitFor(() => expect(fetch).toHaveBeenCalledTimes(2));
    expect(screen.getAllByRole("listitem")).toHaveLength(3);
  });

  it("shows every product again after clicking Show All", async () => {
    mockApi({
      [API]: allProducts,
      [`${API}/search?term=lap`]: [allProducts[0]],
    });
    const user = userEvent.setup();
    render(<App />);
    await screen.findByText("Mouse - $30 - Tech");

    await user.type(screen.getByPlaceholderText("Search product"), "lap");
    await user.click(screen.getByRole("button", { name: "Search" }));
    await vi.waitFor(() => expect(screen.getAllByRole("listitem")).toHaveLength(1));

    await user.click(screen.getByRole("button", { name: "Show All" }));

    await vi.waitFor(() => expect(screen.getAllByRole("listitem")).toHaveLength(3));
    expect(fetch).toHaveBeenCalledTimes(3);
  });
});

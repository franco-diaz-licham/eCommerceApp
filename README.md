<h1>🛒 E‑Commerce Web App</h1>

<p><strong>E‑Commerce Web App</strong> is a full‑stack shopping application built with a <strong>.NET 8 Web API</strong> backend and a <strong>React + Vite + TypeScript</strong> frontend. 
It follows <strong>Clean Architecture</strong> to keep controllers thin, business logic isolated in services, and infrastructure concerns neatly separated. 
Core use cases include browsing products with rich filtering, managing a basket, applying coupon codes, and preparing secure checkout with Stripe.</p>

<blockquote>⚠️ <em>Note: CI/CD & containerization are on the roadmap.</em></blockquote>

<hr />

<h2>🚀 Features</h2>
<ul>
  <li>🧭 <strong>Products</strong> – Filter, sort, and paginate products; browse by brand and product type.</li>
  <li>🛒 <strong>Basket</strong> – Add/remove items, update quantities, and persist basket state.</li>
  <li>🏷️ <strong>Coupons</strong> – Apply/remove promotion codes (integrates with Stripe Promotion Codes).</li>
  <li>💳 <strong>Checkout Prep</strong> – Server creates/updates a Stripe <code>PaymentIntent</code> for secure payment flow.</li>
  <li>🖼️ <strong>Media</strong> – Product photos stored via Cloudinary.</li>
  <li>🧱 <strong>Clean Architecture</strong> – API (controllers), Application (use cases/DTOs), Domain (entities), Infrastructure (EF Core, services).</li>
  <li>🧩 <strong>Extensible</strong> – Room for admin CRUD, order workflows, and auth (ASP.NET Identity already wired).</li>
</ul>

<hr />

<h2>🧰 Technology Stack</h2>

<h3>🖥️ Backend – .NET 8 Web API</h3>
<ul>
  <li><strong>ASP.NET Core</strong> – Minimal, high‑performance REST API.</li>
  <li><strong>Entity Framework Core</strong> – ORM with code‑first migrations.</li>
  <li><strong>ASP.NET Identity</strong> – IdentityDbContext foundation (ready for auth flows).</li>
  <li><strong>AutoMapper</strong> – DTO mapping.</li>
  <li><strong>Stripe .NET</strong> – PaymentIntent & promotion code lookups.</li>
  <li><strong>Cloudinary</strong> – Image storage for products.</li>
  <li><strong>xUnit (planned)</strong> – Unit tests for services and controllers.</li>
</ul>

<h3>🌐 Frontend – React + Vite + TypeScript</h3>
<ul>
  <li><strong>React 18/19‑ready + Vite</strong> – Fast dev server and modern build tool.</li>
  <li><strong>Redux Toolkit + RTK Query</strong> – State + API data fetching.</li>
  <li><strong>React Router</strong> – Client‑side routing.</li>
  <li><strong>Material UI (MUI)</strong> – Accessible, responsive UI components.</li>
  <li><strong>react‑toastify</strong> – User feedback & notifications.</li>
</ul>

<hr />

<h2>📦 Project Architecture</h2>
<p><em>Clean Architecture layout</em></p>

<h3>🧠 Backend</h3>
<pre>
Src/
├── Api/                 # Controllers, middleware, HTTP models
│   ├── Enpoints/        # Products, Brands, ProductTypes, Basket, etc.
│   ├── Helpers/         # HttpExtensions (pagination headers, results → HTTP)
│   └── Models/          # Request/Response contracts
├── Application/         # DTOs, interfaces, queries, mapping
│   ├── DTOs/
│   ├── Interfaces/      # IBasketService, IProductService, IPaymentService...
│   ├── Mappings/
│   └── Queries/         # Filtering, sorting, search strategies
├── Domain/              # Entities, value objects, events, enums
└── Infrastructure/      # EF Core DbContext, migrations, services (Stripe, Cloudinary)
    ├── Persistence/
    ├── Services/
    └── Seed/
</pre>

<h3>🖥️ Frontend</h3>
<pre>
src/
├── app/
│   ├── main.tsx         # App entry
│   ├── App.tsx
│   ├── routes/          # Route definitions
│   ├── providers/       # RTK Query base API, global providers
│   └── store/           # Redux store & hooks
├── components/          # Layout, UI primitives
├── entities/            # Brand, ProductType, OrderStatus models
├── features/            # Product & Error modules (pages, components, services)
├── hooks/               # Reusable hooks
├── lib/                 # Utilities (photo, etc.)
├── assets/              # Static assets
└── styles/              # Global CSS
</pre>

<hr />

<h2>🗄️ Database</h2>

<p align="center">
  <img src="docs/db-diagram.png" alt="ERD diagram" />
</p>

<ul>
  <li><strong>Products</strong> belong to a <em>Brand</em> and a <em>ProductType</em>, and may reference a <em>Photo</em>.</li>
  <li><strong>Baskets</strong> hold many <em>BasketItems</em> (ProductId, UnitPrice, Quantity) and can reference a <em>Coupon</em>.</li>
  <li><strong>Orders</strong> reference an <em>OrderStatus</em> and contain many items via the <em>OrderItem</em> bridge to <em>ProductItem</em>.</li>
  <li><strong>ProductItem</strong> captures a snapshot (price/qty) of a product at purchase time to preserve historical correctness.</li>
  <li><strong>Photos</strong> are stored separately and linked to products.</li>
</ul>

<hr />

<h2>🔌 API Endpoints (at a glance)</h2>
<pre>
GET    /api/products
GET    /api/products/{id}

GET    /api/brands
GET    /api/producttypes
GET    /api/orderstatuses

POST   /api/basket                 # Create a new basket
GET    /api/basket/{id}            # Get current basket
POST   /api/basket/add-item        # Add or increment item
DELETE /api/basket/remove-item     # Remove or decrement item
POST   /api/basket/add-coupon/{code}
DELETE /api/basket/remove-coupon
</pre>

<hr />

<h2>🏁 Getting Started</h2>

<h3>Prerequisites</h3>
<ul>
  <li>.NET 8 SDK</li>
  <li>Node.js 20+</li>
  <li>SQL database (SQL Server or PostgreSQL)</li>
  <li>Stripe account (test keys)</li>
  <li>Cloudinary account (for image uploads)</li>
</ul>

<h3>Backend</h3>
<ol>
  <li>Create <code>appsettings.Development.json</code> in <code>Src/Api</code> (or API project root):</li>
</ol>

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ShopDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "CloudinarySettings": {
    "CloudName": "YOUR_CLOUD_NAME",
    "ApiKey": "YOUR_API_KEY",
    "ApiSecret": "YOUR_API_SECRET"
  },
  "StripeSettings": {
    "PublishableKey": "pk_test_xxx",
    "SecretKey": "sk_test_xxx"
  }
}
```

<ol start="2">
  <li>Apply migrations & seed (if applicable): <code>dotnet ef database update</code></li>
  <li>Run the API: <code>dotnet run</code> (note the HTTPS port)</li>
</ol>

<h3>Frontend</h3>
<ol>
  <li>Create <code>.env</code> at the client root:</li>
</ol>

```bash
VITE_API_URL=https://localhost:5001/api
# VITE_STRIPE_PUBLISHABLE_KEY=pk_test_xxx   # if/when used on the client
```

<ol start="2">
  <li>Install deps: <code>npm install</code></li>
  <li>Start dev server: <code>npm run dev</code></li>
</ol>

<hr />

<h2>🧪 Testing (roadmap)</h2>
<ul>
  <li>Service‑level unit tests with xUnit and FluentAssertions</li>
  <li>API integration tests (WebApplicationFactory)</li>
  <li>Frontend component tests with React Testing Library</li>
</ul>

<hr />

<h2>📈 Future Enhancements</h2>
<ul>
  <li>Order creation & payment confirmation workflow</li>
  <li>JWT‑based authentication & authorization</li>
  <li>Admin UI for product/brand/type management</li>
  <li>Dockerfile + docker‑compose for local orchestration</li>
  <li>CI/CD via Azure Pipelines or GitHub Actions</li>
</ul>

<hr />

<h2>📜 License</h2>
<p>MIT (or your preferred license)</p>

name: backend-dev
description: Implements LifeUniform .NET 10 Razor Pages shop features using clean layered architecture without CQRS. Use proactively when adding pages, CRUD, catalog, cart, orders, account, admin, EF Core data access, validation, Bootstrap UI.

You are a developer for LifeUniform: .NET 10, ASP.NET Core Razor Pages, Bootstrap, EF Core, clean layered architecture. Use this skill when implementing shop features.

Main rule
Use clean layered architecture without CQRS/MediatR:
- Domain: domain model.
- Application: DTOs, interfaces, validators, application services.
- Infrastructure: EF Core, repositories, external services.
- Web: Razor Pages, PageModels, UI, Bootstrap.

Default stack
- .NET 10
- ASP.NET Core Razor Pages
- Bootstrap 5
- EF Core
- FluentValidation
- xUnit
- dotnet publish artifacts
- No Docker

Project
Solution: LifeUniform.
Web project: LifeUniform.Web.
If the actual project name differs, use the actual name.

Modules
Catalog: products, categories, brands, filters, search, pagination, product details.
Cart: cart, quantities, totals, coupons.
Orders: checkout, order history, statuses, payment statuses.
Account: profile, addresses, password change, email confirmation.
Admin: product/category/order/user management, image upload.
Files: product images, avatars, documents.

Architecture rules
Domain
- Business model only.
- No EF Core, HTTP, Razor Pages, Bootstrap.
- Rich entities with private setters and factory methods.
- Money as decimal or Money value object.

Application
- Use-case services: CatalogService, CartService, OrderService.
- DTOs: ProductDto, CartDto, OrderDto, CreateProductInput.
- Interfaces: ICatalogService, ICartService, IOrderService.
- Validators: CreateProductValidator, CheckoutValidator.
- No DbContext and no Razor Page dependencies.

Infrastructure
- Implements Application interfaces.
- Contains DbContext, configurations, repositories, external adapters.
- Use fluent configuration, not data annotations.

Web
- Razor Pages calls Application services only.
- PageModels are thin.
- UI uses Bootstrap 5.
- Use tag helpers and ModelState.
- Use partials for repeated markup.
- Use ViewComponents for dynamic reusable widgets.

Razor Pages conventions
- Pages/Catalog/Index.cshtml — product list.
- Pages/Catalog/Details.cshtml — product page.
- Pages/Cart/Index.cshtml — cart.
- Pages/Orders/Checkout.cshtml — checkout.
- Pages/Account/Login.cshtml — login.
- Pages/Admin/Products/Index.cshtml — admin product list.
- OnGetAsync reads data through services.
- OnPostAsync validates input, calls service, redirects on success.

Validation
- Use FluentValidation.
- Validators live in Application.
- PageModels validate before calling services.
- Validation errors go into ModelState.

UI/Bootstrap
- Use standard Bootstrap 5 classes.
- Forms: form-label, form-control, form-select, form-check.
- Buttons: btn, btn-primary, btn-outline-secondary.
- Alerts: alert, alert-success, alert-danger.
- Tables: table, table-striped, table-hover.
- Cards: card, card-body, card-img-top.
- Do not add custom CSS unless required.

Images
- Do not store uploaded images in source code.
- Validate file type and size.
- Prefer storage outside webroot or protected file endpoint.
- Generate thumbnails if image processing is configured.

Money and orders
- Prices stored as decimal.
- Do not use double for money.
- Order totals are calculated server-side.
- Client-side totals are display-only.
- Order statuses are enum or reference data.

Security
- POST forms use anti-forgery tokens.
- Admin pages require authorization.
- Do not log sensitive data.
- Do not store passwords in plain text.
- Use Identity defaults for password hashing, lockout, and confirmation when Identity is used.

EF Core
- Use AsNoTracking for list reads.
- Include only required navigation properties.
- Use pagination for product/order/user lists.
- Add indexes for Slug, Email, OrderNumber, ProductId.

Testing
- Unit test application services.
- Unit test validators.
- Unit test domain logic.
- Add integration tests for critical pages when required.

Publishing
- Do not use Docker.
- Release build:
  dotnet publish src/LifeUniform.Web -c Release -o artifacts/publish/LifeUniform.Web
- CI can archive the artifacts folder.
- Do not create Dockerfile/docker-compose unless explicitly requested.

Forbidden
- MediatR/CQRS unless explicitly requested.
- Mixing HTTP logic with domain logic.
- Returning domain entities directly to Razor Pages.
- Business logic in PageModels.
- Data annotations on domain entities instead of EF fluent configuration.
- Docker without explicit request.
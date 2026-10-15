// Visual smoke test for the storefront: walks the shopping flow as the demo buyer and
// the seller pages as the demo seller, saving desktop and 360px-wide screenshots.
import { chromium } from "playwright";
import { mkdir } from "node:fs/promises";

const baseUrl = process.env.BASE_URL ?? "http://localhost:5080";
const out = new URL("./output/", import.meta.url).pathname;
const password = "Demo123!";

const viewports = {
  desktop: { viewport: { width: 1440, height: 900 } },
  mobile: { viewport: { width: 360, height: 780 }, deviceScaleFactor: 2, isMobile: true, hasTouch: true },
};

await mkdir(out, { recursive: true });
const browser = await chromium.launch();
const problems = [];

async function shot(page, name) {
  await page.waitForLoadState("networkidle").catch(() => {});
  const overflow = await page.evaluate(() => document.documentElement.scrollWidth - window.innerWidth);
  if (overflow > 1) problems.push(`${name}: page is ${overflow}px wider than the viewport`);
  await page.screenshot({ path: `${out}${name}.png`, fullPage: true });
  console.log(`saved ${name}.png`);
}

async function login(page, email) {
  await page.goto(`${baseUrl}/login`);
  await page.fill("#Email", email);
  await page.fill("#Password", password);
  await Promise.all([page.waitForNavigation(), page.click("form button[type=submit]")]);
}

for (const [device, options] of Object.entries(viewports)) {
  const context = await browser.newContext({ ...options, locale: "tr-TR" });
  const page = await context.newPage();

  await page.goto(`${baseUrl}/`);
  await shot(page, `${device}-home`);

  await page.goto(`${baseUrl}/product/list`);
  await shot(page, `${device}-listing`);

  await page.goto(`${baseUrl}/product/list?category=3&sort=PriceAsc&discounted=true`);
  await shot(page, `${device}-listing-filtered`);

  await page.goto(`${baseUrl}/product/list?search=zzzz`);
  await shot(page, `${device}-listing-empty`);

  await page.goto(`${baseUrl}/product/list?page=2`);
  await shot(page, `${device}-listing-page-2`);

  await page.goto(`${baseUrl}/product/12/details`);
  await shot(page, `${device}-product`);

  await page.goto(`${baseUrl}/login`);
  await page.click("form button[type=submit]");
  await shot(page, `${device}-login-validation`);

  await page.goto(`${baseUrl}/register`);
  await shot(page, `${device}-register`);

  // Blog: list, tag filter, post page and a reader comment waiting for approval.
  await page.goto(`${baseUrl}/blog`);
  await shot(page, `${device}-blog`);
  await page.goto(`${baseUrl}/blog?tag=6`);
  await shot(page, `${device}-blog-tag`);
  await page.goto(`${baseUrl}/blog/2`);
  await shot(page, `${device}-blog-post`);
  if (device === "desktop") {
    await page.fill("#Form_Name", "Ayşe Yılmaz");
    await page.fill("#Form_Email", "ayse@example.com");
    await page.fill("#Form_Comment", "Narenciyeyi poşetsiz saklamayı denedim, gerçekten daha uzun dayanıyor.");
    await Promise.all([page.waitForNavigation(), page.click("#comments form button[type=submit]")]);
    await shot(page, `${device}-blog-comment-sent`);

    // Password reset: without SMTP the Development build shows the link on the page.
    await page.goto(`${baseUrl}/forgot-password`);
    await page.fill("#Email", "elif.arslan@example.com");
    await Promise.all([page.waitForNavigation(), page.click("form button[type=submit]")]);
    await shot(page, `${device}-forgot-password-sent`);
    await Promise.all([page.waitForNavigation(), page.click(".alert-warning a")]);
    await shot(page, `${device}-renew-password`);
    await page.fill("#Password", password);
    await page.fill("#PasswordConfirm", password);
    await Promise.all([page.waitForNavigation(), page.click("form button[type=submit]")]);
    await shot(page, `${device}-renew-password-done`);
  }

  // Buyer: add to cart, check out, see the order.
  await login(page, "buyer@example.com");
  for (const id of [12, 2, 30]) {
    await page.goto(`${baseUrl}/add-to-cart/${id}`);
  }
  await page.goto(`${baseUrl}/cart`);
  await shot(page, `${device}-cart`);

  if (device === "desktop") {
    await Promise.all([page.waitForNavigation(), page.click(".qty-form [data-qty-step='1'] >> nth=0")]);
    await shot(page, `${device}-cart-updated`);
  }

  await page.goto(`${baseUrl}/checkout`);
  await page.click("#checkoutForm button[type=submit]");
  await page.waitForLoadState("networkidle").catch(() => {});
  await shot(page, `${device}-checkout-validation`);

  await page.goto(`${baseUrl}/checkout?coupon=YANLIS`);
  await shot(page, `${device}-checkout-coupon-invalid`);

  await page.goto(`${baseUrl}/checkout?coupon=hosgeldin10`);
  await page.fill("#Address", "Bağdat Cad. No: 214 D: 7, Kadıköy / İstanbul");
  await shot(page, `${device}-checkout`);
  await Promise.all([page.waitForNavigation(), page.click("#checkoutForm button[type=submit]")]);
  await shot(page, `${device}-order-confirmed`);

  await page.goto(`${baseUrl}/my-orders`);
  await shot(page, `${device}-my-orders`);

  await page.goto(`${baseUrl}/order/MK-2026-0004/details`);
  await shot(page, `${device}-order-shipped`);

  await page.goto(`${baseUrl}/profile`);
  await shot(page, `${device}-profile`);

  await page.goto(`${baseUrl}/logout`);

  // Seller pages.
  await login(page, "seller@example.com");
  await page.goto(`${baseUrl}/my-products`);
  await shot(page, `${device}-my-products`);

  await page.goto(`${baseUrl}/product`);
  await shot(page, `${device}-product-create`);

  if (device === "mobile") {
    await page.goto(`${baseUrl}/`);
    await page.click(".header-actions .dropdown button");
    await page.waitForTimeout(300);
    await page.screenshot({ path: `${out}${device}-account-menu.png` });
    console.log(`saved ${device}-account-menu.png`);
  }

  await page.goto(`${baseUrl}/does-not-exist`);
  await shot(page, `${device}-not-found`);

  await context.close();
}

// Admin panel.
const adminUrl = process.env.ADMIN_URL ?? "http://localhost:5081";
for (const [device, options] of Object.entries(viewports)) {
  const context = await browser.newContext({ ...options, locale: "tr-TR" });
  const page = await context.newPage();

  await page.goto(`${adminUrl}/login`);
  await shot(page, `${device}-admin-login`);
  await page.fill("#Email", "admin@example.com");
  await page.fill("#Password", password);
  await Promise.all([page.waitForNavigation(), page.click("form button[type=submit]")]);
  await shot(page, `${device}-admin-dashboard`);

  await page.goto(`${adminUrl}/products`);
  await shot(page, `${device}-admin-products`);
  await page.goto(`${adminUrl}/users?onlySellerRequests=true`);
  await shot(page, `${device}-admin-seller-requests`);
  await page.goto(`${adminUrl}/comments`);
  await shot(page, `${device}-admin-comments`);
  await page.goto(`${adminUrl}/categories`);
  await shot(page, `${device}-admin-categories`);
  await page.goto(`${adminUrl}/orders`);
  await shot(page, `${device}-admin-orders`);
  await page.goto(`${adminUrl}/orders/5`);
  await shot(page, `${device}-admin-order`);
  await page.goto(`${adminUrl}/coupons`);
  await shot(page, `${device}-admin-coupons`);
  await page.goto(`${adminUrl}/blog-comments`);
  await shot(page, `${device}-admin-blog-comments`);

  if (device === "desktop") {
    // Bulk action: select two products and take them off sale, through the confirm dialog.
    await page.goto(`${adminUrl}/products?search=Muz`);
    await page.check("input[data-bulk-all]");
    await shot(page, `${device}-admin-products-selected`);
    await page.click("button[value=disable]");
    await page.waitForTimeout(400);
    await shot(page, `${device}-admin-confirm`);
    await Promise.all([page.waitForNavigation(), page.click("[data-confirm-accept]")]);
    await shot(page, `${device}-admin-products-disabled`);

    // Approve the pending seller request and every pending comment.
    await page.goto(`${adminUrl}/users?onlySellerRequests=true`);
    await Promise.all([page.waitForNavigation(), page.click("form[action$='/approve'] button")]);
    await page.goto(`${adminUrl}/comments`);
    await page.check("input[data-bulk-all]");
    await Promise.all([page.waitForNavigation(), page.click("button[value=approve]")]);
    await shot(page, `${device}-admin-comments-approved`);

    // Move an order forward, then try the coupon form.
    await page.goto(`${adminUrl}/orders/5`);
    await Promise.all([page.waitForNavigation(), page.click("input[name=status][value=Shipped] ~ button")]);
    await shot(page, `${device}-admin-order-shipped`);

    await page.goto(`${adminUrl}/coupons/new`);
    await page.fill("#Code", "x");
    await page.click("form button[type=submit]");
    await page.waitForTimeout(300);
    await shot(page, `${device}-admin-coupon-validation`);
    await page.fill("#Code", "bahar25");
    await page.fill("#DiscountRate", "25");
    await page.fill("#UsageLimit", "100");
    await Promise.all([page.waitForNavigation(), page.click("form button[type=submit]")]);
    await shot(page, `${device}-admin-coupon-created`);

    await page.goto(`${adminUrl}/blog-comments`);
    await page.check("input[data-bulk-all]");
    await Promise.all([page.waitForNavigation(), page.click("button[value=approve]")]);
    await shot(page, `${device}-admin-blog-comments-approved`);
  }

  await context.close();
}

await browser.close();

if (problems.length > 0) {
  console.error("Layout problems:\n" + problems.join("\n"));
  process.exit(1);
}

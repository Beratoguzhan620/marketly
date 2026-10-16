// Regenerates the README images (docs/screenshots) from the seeded demo data.
// Needs the storefront and the admin panel running. The walkthrough video is saved as
// output/media/walkthrough.webm; CI turns it into docs/screenshots/demo.gif with ffmpeg.
import { chromium } from "playwright";
import { mkdir, readdir, rename } from "node:fs/promises";

const baseUrl = process.env.BASE_URL ?? "http://localhost:5080";
const adminUrl = process.env.ADMIN_URL ?? "http://localhost:5081";
const out = new URL("./output/media/", import.meta.url).pathname;
const password = "Demo123!";
const pause = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

await mkdir(out, { recursive: true });
const browser = await chromium.launch();

async function login(page, url, email) {
  await page.goto(url);
  await page.fill("#Email", email);
  await page.fill("#Password", password);
  await Promise.all([page.waitForNavigation(), page.click("form button[type=submit]")]);
}

async function shot(page, name) {
  await page.waitForLoadState("networkidle").catch(() => {});
  await page.screenshot({ path: `${out}${name}.png` });
  console.log(`saved ${name}.png`);
}

// Still images, desktop.
{
  const context = await browser.newContext({ viewport: { width: 1280, height: 800 }, locale: "tr-TR" });
  const page = await context.newPage();

  await page.goto(`${baseUrl}/`);
  await shot(page, "home");
  await page.goto(`${baseUrl}/product/list?category=3`);
  await shot(page, "listing");
  await page.goto(`${baseUrl}/product/12/details`);
  await shot(page, "product");

  await login(page, `${baseUrl}/login`, "buyer@example.com");
  for (const id of [12, 2, 30]) await page.goto(`${baseUrl}/add-to-cart/${id}`);
  await page.goto(`${baseUrl}/cart`);
  await shot(page, "cart");
  await page.goto(`${baseUrl}/checkout?coupon=HOSGELDIN10`);
  await page.fill("#Address", "Bağdat Cad. No: 214 D: 7, Kadıköy / İstanbul");
  await shot(page, "checkout");
  await page.goto(`${baseUrl}/order/MK-2026-0004/details`);
  await shot(page, "order");
  await page.goto(`${baseUrl}/blog/2`);
  await shot(page, "blog-post");

  await login(page, `${adminUrl}/login`, "admin@example.com");
  await shot(page, "admin-dashboard");
  await page.goto(`${adminUrl}/orders`);
  await shot(page, "admin-orders");
  await page.goto(`${adminUrl}/products`);
  await shot(page, "admin-products");

  await context.close();
}

// Still image, phone.
{
  const context = await browser.newContext({ viewport: { width: 390, height: 844 }, deviceScaleFactor: 2, isMobile: true, hasTouch: true, locale: "tr-TR" });
  const page = await context.newPage();
  await page.goto(`${baseUrl}/product/list?category=3`);
  await shot(page, "mobile-listing");
  await context.close();
}

// Walkthrough video for the README GIF: browse, add to cart, apply a coupon, place the order.
{
  const context = await browser.newContext({
    viewport: { width: 1280, height: 800 },
    locale: "tr-TR",
    recordVideo: { dir: out, size: { width: 1280, height: 800 } },
  });
  const page = await context.newPage();

  await page.goto(`${baseUrl}/login`);
  await page.type("#Email", "elif.arslan@example.com", { delay: 20 });
  await page.type("#Password", password, { delay: 20 });
  await Promise.all([page.waitForNavigation(), page.click("form button[type=submit]")]);
  await pause(800);
  await page.goto(`${baseUrl}/product/list?category=3`);
  await pause(1400);
  await page.goto(`${baseUrl}/product/12/details`);
  await pause(1400);
  await page.goto(`${baseUrl}/add-to-cart/12`);
  await page.goto(`${baseUrl}/add-to-cart/30`);
  await page.goto(`${baseUrl}/cart`);
  await pause(1400);
  await page.goto(`${baseUrl}/checkout`);
  await pause(600);
  await page.type("#coupon", "HOSGELDIN10", { delay: 40 });
  await Promise.all([page.waitForNavigation(), page.click(".coupon-form button")]);
  await pause(1000);
  await page.type("#Address", "Alsancak Mah. Kıbrıs Şehitleri Cad. No: 8 Konak / İzmir", { delay: 12 });
  await pause(500);
  await Promise.all([page.waitForNavigation(), page.click("#checkoutForm button[type=submit]")]);
  await pause(2000);

  await context.close();

  const video = (await readdir(out)).find((name) => name.endsWith(".webm"));
  if (video) await rename(`${out}${video}`, `${out}walkthrough.webm`);
}

await browser.close();

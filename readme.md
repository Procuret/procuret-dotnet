# Procuret .NET

A Microsoft .NET library for interfacing with the Procuret API.

## Contents

1. .NET API Compatibility
2. Installation
3. Documentation
4. Support

## .NET API Compatibility

Procuret .NET targets .NET 8.0. If you require comptatiblity with
an earlier .NET release, please [contact us](mailto:support@procuret.com).

## Installation

Procuret .NET is available as a
[Nuget package](https://www.nuget.org/packages/ProcuretAPI/).

```bash
dotnet add package ProcuretAPI
```

See Microsoft's documentation for more information about installing Nuget
packages in Visual Studios on [Windows](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio) and
[macOS](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio-mac).

If you require assistance installing Procuret .NET in your development
environment, please [contact us](mailto:support@procuret.com).

## Documentation

Procuret .NET offers a library of classes that map to services provided
by the Procuret API.

### `Session`

Sessions are the means of authenticating requests to the Procuret API. All
requests to Procuret API, save for those creating Sessions themselves, require
a Session.

In Procuret .NET, the `Session` class will handle all authentication for you.
For example, it will compute the SHA256 signature that must be included
in your HTTP headers.

#### Initialiser

##### Parameters

1. `String` apiKey
2. `ulong` sessionId

##### Example Initialisation

```cs
Session session = Session(
    apiKey: "your_api_key",
    sessionId: 441121225 // Your Session ID
)
```

### `struct InstalmentLink`

`InstalmentLink` facilitates the creation of customised links to the Procuret
Instalment Product (PIP). PIP allows a customer Business to pay for a purchase
over time, while you the Supplier are paid upfront.

When you create an `InstalmentLink`, you can ask Procuret to send an email
to the customer Business on your behalf.

To create a new `InstalmentLink`, call `InstalmentLink.Create()`. `.Create()`
operates asynchronously, so use the `await` keyword to initalise the link
into a variable.

#### Properties

1. `String` PublicId - unique ID of this `InstalmentLink`
2. `EntityHeadline` Supplier - the Supplier to which the link applies
3. `Decimal` InvoiceAmount - the amount the link attempts to transact
4. `String` InvoiceIdentifier - the identifier of the invoice
5. `String` InviteeEmail - the email address associated with the link
6. `Currency` Denomination - the currency in which the link is denominated

#### Methods

##### `static async Task<InstalmentLink>.Create(...)`

###### Parameters

1. `long` supplierId - Your unique Supplier ID
2. `String` customerEmail - The email of the customer to whom a Procuret
payment invitation email should be sent.
3. `String` invoiceIdentifier - Your invoice identifier. E.g. `"INV-001"`
4. `Decimal` invoiceValue - The amount payable by the customer
5. `CommunicationOption` communication - Instance of `CommunicationOption`
enumeration
6. `Session` session - The `Session` to use when authenticating your request
7. `Currency` denomination - The currency in which the link should be
denominated

###### Example Usage

```cs
var link = await InstalmentLink.Create(
    supplierId: 589121125121,
    customerEmail: "someone@somewhere.com",
    invoiceIdentifier: "INV-001",
    invoiceValue: Convert.ToDecimal("422.22"),
    communciation: CommunicationOption.NotifyCustomer,
    session: session, // See Session create example above
    denomination: Currency.Aud
)
```

### `struct ProspectivePayment`

A `ProspectivePayment` represents the amount of money that a customer may
pay, per payment period, should they use the Procuret Instalment Product to pay
for their purchase.

In other words, you can use `ProspectivePayment` to give customers a preview
of the cost of using Procuret. Retrieve a `ProspectivePayment` for any
number of periods. At this time, Procuret .NET only supports monthly payments.

#### Properties

1. `Decimal` RecurringPayment - The amount the customer will pay per period
2. `long` SupplierId - The ID of the Supplier to which thise price applies
3. `Int16` PaymentCount - The number of payments the customer would make
4. `Period` Period - The length of the payment period (always `.MONTH`)
5. `Cycle` Cycle - The cycle of the payment (always `.ADVANCE`)

#### Methods

##### `static async Task<ProspectivePayment> Retrieve(...)`

Use .Retrieve() to look up the price a customer would pay per period using
the Procuret Instalment Product.

###### Parameters

1. `Session` session - An instance of `Session` authenticating your request
2. `long` supplierId - Your Supplier ID
3. `Decimal` principle - The total value of the purchase, including GST
4. `Int16` paymentCount - The number of payments the customer would make

###### Example Usage

```cs
var payment = await ProspectivePayment.Retrieve(
    session: session                       // See Session example elsewhere
    supplierId: 589121125121,
    principle: Convert.ToDecimal("4200"),  // Includes GST
    paymentCount: 12                       // Implies 12 monthly payments
);

Console.WriteLine(payment.RecurringPayment.ToString());
```

##### `static async Task<ProspectivePayment[]> RetrieveMany(...)`

###### Parameters

1. `Session` session - An instance of `Session` authenticating your request
2. `long` supplierId - Your Supplier ID
3. `Decimal` principle - The total value of the purchase, including GST

###### Example Usage

```cs
var payments = await ProspectivePayment.RetrieveMany(
    session: session,                      // See Session example elswhere
    supplierId: 589121125121,
    principle: Convert.ToDecimal("4200")   // Includes GST
);

foreach (ProspectivePayment payment in payments)
{
    Console.WriteLine(payment.RecurringPayment.ToString());
}
```

### `struct EntityHeadline`

A type containing basic data about a legal person.

#### Properties

`long` EntityId - A unique Procuret identifier for the legal person
`String` LegalEntityName - The name of the legal person, e.g. "ACME Ltd"

### `enum CommunicationOption`

An enumeration of instructions you can send Procuret in some contexts, to
tell it how you wish for it to contact (or not contact) the a customer.

#### Cases

- `.EMAIL_CUSTOMER` - Procuret will contact the customer by email
- `.DO_NOT_CONTACT_CUSTOMER` - Procuret will not try to contact the customer

### `enum Currency`

An enumeration of currencies in which Procuret transactions may be
denominated.

#### Cases

- `.Aud` - Australian dollars
- `.Nzd` - New Zealand dollars

## Support

Please contact us anytime at [support@procuret.com](mailto:support@procuet.com)
with any questions. To chat with us less formally, please feel free to tweet
[@hugh_jeremy](https://twitter.com/hugh_jeremy).

For more general information about Procuret, please visit
[procuret.com](https://procuret.com).

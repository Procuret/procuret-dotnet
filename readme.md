# Procuret .NET

A Microsoft .NET Standard library for interfacing with the Procuret API.

## Contents

1. .NET API Compatibility
2. Installation
3. Documentation
4. Support

## .NET API Compatibility

Procuret .NET targets .NET Standard 2.0, which means it is compatible with
.NET Framework 4.6.1+ and .NET Core 2.0+. If you require comptatiblity with
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

### `InstalmentLink`

`InstalmentLink` facilitates the creation of customised links to the Procuret
Instalment Product (PIP). PIP allows a customer Business to pay for a purchase
over time, while you the Supplier are paid upfront.

When you create an `InstalmentLink`, you can ask Procuret to send an email
to the customer Business on your behalf.

To create a new `InstalmentLink`, call `InstalmentLink.Create()`. `.Create()`
operates asynchronously, so use the `await` keyword to initalise the link
into a variable.

#### Properties

`String` PublicId - unique ID of this `InstalmentLink`
`EntityHeadline` Supplier - the Supplier to which the link applies
`Decimal` InvoiceAmount - the amount the link attempts to transact
`String` InvoiceIdentifier - the identifier of the invoice
`String` InviteeEmail - the email address associated with the link

#### Methods

##### `static async Task<InstalmentLink>.create(...)`

###### Parameters

1. `Int64` supplierId - Your unique Supplier ID
2. `String` customerEmail - The email of the customer to whom a Procuret
payment invitation email should be sent.
3. `String` invoiceIdentifier - Your invoice identifier. E.g. `"INV-001"`
4. `Decimal` invoiceValue - The amount payable by the customer
5. `CommunicationOption` communication - Instance of `CommunicationOption`
enumeration
6. `Session` session - The `Session` to use when authenticating your request

###### Example Usage

```cs
var link = await InstalmentLink.create(
    supplierId: 589121125121,
    customerEmail: "someone@somewhere.com",
    invoiceIdentifier: "INV-001",
    invoiceValue: Convert.ToDecimal("422.22"),
    communciation: CommunicationOption.NotifyCustomer,
    session: session // See Session create example above
)
```

### `CommunicationOption`

An enumeration of instructions you can send Procuret in some contexts, to
tell it how you wish for it to contact (or not contact) the a customer.

#### Cases

- `.EMAIL_CUSTOMER` - Procuret will contact the customer by email
- `.DO_NOT_CONTACT_CUSTOMER` - Procuret will not try to contact the customer

## Support

Please contact us anytime at [support@procuret.com](mailto:support@procuet.com)
with any questions. To chat with us less formally, please feel free to tweet
[@hugh_jeremy](https://twitter.com/hugh_jeremy).

For more general information about Procuret, please visit
[procuret.com](https://procuret.com).

# Procuret .NET

A Microsoft .NET Standard library for interfacing with the Procuret API.

## Contents

1. .NET API Compatibility
2. Installation
3. Documentation
4. Support

## .NET API Compatibility

Procuret .NET targets .NET Standard 1.6, which means it is compatible with
.NET Framework 4.7.2+ and .NET Core 1.0+. If you require comptatiblity with
an earlier .NET release, please [contact us](mailto:support@procuret.com).

## Installation

_WIP_

## Documentation

Procuret Python offers a library of classes that map to services provided
by the Procuret API.

### `Session`

Sessions are the means of authenticating requests to the Procuret API. All
requests to Procuret API, save for those creating Sessions themselves, require
a Session.

In Procuret Python, the `Session` class will handle all authentication for you.
For example, it will compute the SHA256 signature that must be included
in your HTTP headers.

#### Properties

WIP

#### Methods

WIP

### `InstalmentLink`

`InstalmentLink` facilitates the creation of customised links to the Procuret
Instalment Product (PIP). PIP allows a customer Business to pay for a purchase
over time, while you the Supplier are paid upfront.

When you create an `InstalmentLink`, you can ask Procuret to send an email
to the customer Business on your behalf.

#### Properties

WIP

#### Methods

WIP

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

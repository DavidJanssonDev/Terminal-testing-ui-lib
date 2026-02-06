# Security Policy

## Supported Versions

This project is currently in **early development** and does not have formal release versions yet.

Security updates, if any, will be applied to the `main` branch.

---

## Reporting a Vulnerability

If you discover a security vulnerability, please follow these steps:

1. **Do not open a public issue** describing the vulnerability in detail.
2. Instead, report it responsibly by:
   - Opening a GitHub issue with **minimal details**, or
   - Contacting the maintainer directly if contact information is available.

Please include:

- a clear description of the issue
- steps to reproduce (if applicable)
- potential impact

---

## Scope

Because this is a terminal UI library:

- it does not handle networking
- it does not handle authentication
- it does not manage sensitive user data by default

Most security concerns are expected to relate to:

- terminal escape sequence handling
- input validation
- unexpected behavior in edge cases

---

## Disclosure

This project follows a **best-effort responsible disclosure** approach.

Thank you for helping keep the project safe.

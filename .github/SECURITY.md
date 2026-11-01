# Security Policy

## Reporting a vulnerability

Please **do not** report security vulnerabilities through public GitHub issues, discussions or pull requests.

Instead, please use GitHub's private vulnerability reporting: go to the [Security tab](https://github.com/leekelleher/umbraco-contentment/security) of this repository and select **Report a vulnerability**.

Please include as much of the following as you can, to help me understand and reproduce the issue:

- the version(s) of Contentment and Umbraco affected,
- the type of issue (e.g. path traversal, server-side request forgery, missing authorization),
- the steps to reproduce it, ideally with a proof of concept, and
- the impact you expect it to have.

Contentment is a community package that I maintain in my own time, so I can't promise a fixed response time. I will acknowledge your report, keep you updated while I work on a fix, and credit you in the published advisory if you would like to be named.

## Supported versions

Security fixes are released for the latest version of each major release line that targets a supported version of Umbraco.

| Contentment | Umbraco |
| ----------- | ------- |
| 7.x         | 18      |
| 6.x         | 16, 17  |
| 5.x         | 13      |

Older versions don't receive security fixes. Please upgrade to a supported version.

## Disclosure

I follow a coordinated disclosure process. Once a fix is released, a security advisory is published on this repository, with a CVE identifier where one has been assigned.

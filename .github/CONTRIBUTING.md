This project is governed by a [Code of Conduct](CODE_OF_CONDUCT.md).

### General gist

Before writing any code, [please open an issue](https://github.com/leekelleher/umbraco-contentment/issues), or [let's have a discussion](https://github.com/leekelleher/umbraco-contentment/discussions) about any features or ideas that you may have.

> **No-nos!** Saying this upfront, please do not upgrade any NuGet dependencies and please do not modify any licensing information, e.g. do not update the copyright year.
> I have strong opinions on how I like these to be managed.

Okay, once you have the feature or idea is fleshed out, it's hacking time!

1. Fork it
2. Branch it
3. Hack it
4. Save it
5. Commit it
6. Push it
7. Pull-request it

> The above is respectively taken from the [Clearwater framework repository](https://github.com/clearwater-rb/clearwater/blob/master/README.md#contributing).<br>
> Kudos to [Jamie Gaskins](https://github.com/jgaskins) for framing the guidelines so succinctly.


### Branching information

Given there are now multiple versions of Contentment that support multiple versions of Umbraco, please make note of the branching structure.

- The main [`develop`](https://github.com/leekelleher/umbraco-contentment/tree/develop) branch is where the latest work happens. This defaults to the most recent version of Contentment, (at the time of writing, this is v3.x).
- The [`dev/v1.x`](https://github.com/leekelleher/umbraco-contentment/tree/dev/v1.x) branch is for Contentment **v1.4.x** patch releases, this targets Umbraco **v8.6.1**.
- The [`dev/v2.x`](https://github.com/leekelleher/umbraco-contentment/tree/dev/v2.x) branch is for Contentment **v2.2.x** patch releases, this targets Umbraco **v8.14.0**.
- The [`dev/v3.x`](https://github.com/leekelleher/umbraco-contentment/tree/dev/v3.x) branch is for Contentment **v3.4.x** patch releases, this targets both Umbraco **v8.17.0** and **v9.0.0**.
- The [`dev/v4.x`](https://github.com/leekelleher/umbraco-contentment/tree/dev/v4.x) branch is for Contentment **v4.x** (current) releases, this targets Umbraco **v8.17.0**, **v9.5.0**, **v10.0.0**, **v11.0.0** and **v12.0.0**.
- The [`dev/v5.x`](https://github.com/leekelleher/umbraco-contentment/tree/dev/v5.x) branch will be for Contentment **v5.x** (next) releases, this will target Umbraco **v10.3.0**, **v11.0.0**, **v12.0.0** and **v13.0.0**.
- The `dev/v6.x` branch will be for Contentment **v6.x** (future) releases, this will target Umbraco **v15.0.0**.
- The `dev/v7.x` branch will be for Contentment **v7.x** (future) releases, this will target Umbraco **v15.0.0**,**v16.0.0** and **v17.0.0**.


### Further reading

I've been thinking a lot about Jeff Geerling's post ["Why I close PRs (OSS project maintainer notes)"](https://www.jeffgeerling.com/blog/2016/why-i-close-prs-oss-project-maintainer-notes) lately.
If you do submit a PR and feel that I'm closing down the conversation, this is most likely the rationale behind it.


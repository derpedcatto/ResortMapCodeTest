# Resort Map

## Prerequisites

- .NET SDK
- Node.js + npm
- Bash (Linux/macOS; on Windows use Git Bash or WSL)

## Run

This app starts the ASP.NET Core backend and the React web frontend together with one command.

From the project root:

```bash
./run.sh
```

Optional arguments:

```bash
./run.sh --map path/to/map.ascii --bookings path/to/bookings.json
```

- `--map` — ASCII resort map file (default: `map.ascii` in the project root)
- `--bookings` — bookings/guest data file (default: `bookings.json` in the project root)

Relative paths are resolved against the project root.

The script checks both files exist, installs frontend dependencies, then starts:

- Backend: `http://localhost:5012`
- Frontend: `http://localhost:5173`

Press `Ctrl+C` to stop both processes.

## Creation Process

It was a challenge to try and keep a balance between choosing where to go with barebones, simple solutions and where to add an abstraction layer, but I hope I found a good balance here. I thought it would be a bad showcase of my skills to have this app be 'too' simple, but I hope I didn't go overboard in some places!

### Backend

I structured the backend using Pseudo-Onion Architecture, because I find its layer distribution pretty convenient! Thin controllers that only serve as a bridge between server logic and HTTP, a 'Service' layer for all the business logic, an 'Infrastructure' layer for all the database requests, and a 'Domain' for pure business data. I did not go all-in on this architecture here (as it would create some redundant abstractions), so I compacted it a little bit. If it were a real app:

- 'Controllers' would stay the same;
- 'Services' would be in the 'Application' layer (and some utility classes like File Readers would be in their own separate 'Helpers' layer);
- the 'Infrastructure' layer would be an EF Core / Database-only layer (this app simulates it by having it load file data to be called in the 'Services' layer);
- the 'Models' folder would be in 'Domain', and I would also think about creating separate DTO models to avoid exposing unneeded data to the client;
- 'Common' is a bit of a 'for everything else' folder in this project, so I would move classes from there to the different layers where they belong more.

I also chose to go with the Result pattern here, as I like the idea of using exceptions only when it's actually an exception, and for expected potential errors, passing them in Result objects seems clean. I also set my Result class in GlobalExceptionHandler, so only this model would be passed to the client (by default, ASP.NET sends its own data models, which also have generic code that I can't change, and having two separate response models is not good in my opinion).

For simple apps, I would keep it simple; architecture like this isn't needed everywhere.

### Frontend

I'm used to Atomic Design, so I structured the app around it (for me personally, it works pretty well for building in React!). I'm also a fan of SCSS and Modules, especially the setup that you can create with mixins and class extensions. Modern CSS is pretty capable now, but SCSS still has better support for older browsers, so that's a bonus point for it. I omited CSS Layers for simplicity though, but I would add it in a serious project.

I'd say that the biggest pain point of this whole project was writing client tests. I think they came out somewhat rough and would definitely benefit from a more senior look or involvement. Writing tests overall is a struggle for me for some reason, even though I see clear benefits in them. Well, I guess I will get around to them with time.

Other potential problems or things I'd improve:

- The client doesn't validate the map that gets sent from the server and assumes that the passed data is always correct, as the server would outright discard any invalid map. I don't know if it's a problem per se — it was just my decision to have it like that, that's why there are no additional checks in Client;
  I did not add elaborate error descriptions to display;
- I did not add any fallback CSS (but I was not using many modern features, so it should be fine);
- Accessibility is a bit barebones;
- Pool image is unused, as I couldn't find a use for it. Only water sprite is used. I thought about having a calculation, where it would draw pool sprite over water if there is enough space to place it, but I spent time coding elsewhere;
- Not related to Client specifically, but some commits got pretty big with a lot of changes, I should've separated them;

# **Urban Entrepreneur (2024)**

Large Colony/Management Simulator • Published on Steam (April 2024)

*Steam Page:* https://store.steampowered.com/app/2648080/Urban_Entrepreneur

*Availability:* Paid (Steam key available upon request)

# **⭐ Project Overview**

Urban Entrepreneur is a large-scale colony/management simulator inspired by titles like RimWorld—but reimagined through the lens of modern retail psychology and real-world sales tactics used by industry giants such as Walmart. The project was the major focus of my 2023–2024 development year and represents the beginning of my high standards in architecture, gameplay system design, and production-ready engineering practices.

Development began shortly after Clay Soldiers with the goal of preparing a polished prototype for the University of Tulsa Global Game Jam Competition. The prototype won 1st Place in the 2023 Professional Category, open worldwide to all non-student participants. Before awards were announced, I hosted a booth for two days; despite having no tutorial at the time, the game retained players for 30–60 minutes, proving the strength and clarity of its core systems.

*Urban Entrepreneur released on Steam in April 2024.*

# **🎮 Gameplay Summary**

## **Start Screen & Progression**

### Players begin with:

-A cinematic, pre-recorded start menu

-10 base maps

-3 base difficulties

-Map completion tracking by difficulty

### Upon selecting the first map, players enter a barren store with one worker—the Manager—and are guided through a comprehensive interactive tutorial that demonstrates:

-Building shelves, structures, and checkout machines

-Hiring employees and scheduling shifts

-Managing employee stress and personality traits

-Understanding the calendar and seasonal events

-Ordering stock with varying decay rates, values, and seasonal demands

-Adjusting play speed via a custom Tick System

## *Core Loop*

### The simulation runs on a detailed economy and behavioral ecosystem:

-Employees (manager, stockers, janitors, engineers, boss) have stress, traits, job roles, and schedules

-Customers individually search for desired items, shop based on needs, respond to environment, and use self-checkout

-Stockers and managers restock shelves based on store settings and item popularity

-Weather, random events, store policies, and marketing influence customer volume and behavior

-Difficulty escalates through more complex items, demand curves, and external factors

*To win a level, the player must complete map-specific objectives—such as earning a cumulative $100,000 on Map 1.*

## **Advanced Systems**

### Later maps introduce:

-Competitor stores

-Heating & cooling systems

-Store temperature control

-Adjustable music & volume (affecting customer behavior)

-Seasonal demand curves

-Advertising strategies & marketing boosts

-Store hours customization

-Multiple new items across categories (electronics, clothing, produce, etc.)

# **🧩 Key Features**

-Large-scale colony/retail simulation

-Fully moddable data system (maps, items, names, structures, etc.)

-Mod folders auto-generated on startup

-JSON-based save/load system (player-editable if desired)

-Advanced employee and customer state machines

-Point-based AI decision system

-Custom A* pathfinding

-Memory system for customers (stores past item locations)

-Animated and resolution-independent UI system

-Tick-based gameplay loop to control simulation pacing

-Complex economy with demand decay, product lifespans, and variable buy/sell prices

# **🏗️ Architecture Overview**

**Urban Entrepreneur marks one of my most advanced architectural achievements prior to 2025. It introduced multiple high-complexity systems that interact simultaneously across hundreds of agents and objects.**

## **Major Architectural Highlights**

**State Machines**

Employees, customers, and officers use layered state machines to handle searching, shopping, waiting, routing, work tasks, and breaks.

**A Pathfinding**

Used extensively to ensure believable navigation in crowded store environments.

**Tick System**

### A custom timing system controlling:

-Game clock advancement

-Customer/employee updates

-Product decay

-Event triggers

-Stocking logic

*(Note: Later levels can produce lag spikes when too many agents update simultaneously—this is a key target for future refactoring.)*

## *Point-Based AI*

### Characters evaluate actions based on weighted desirability, including:

-Work tasks

-Item retrieval

-Checkout decisions

-Employee stress & break behavior

**-Memory System**

Customers remember item positions and use that information in subsequent searches.

## **Modding / Data System**

### A folder is auto-created on startup containing JSON templates for:

-Items

-Structures

-Maps

-Names

-Store settings

*Users can edit these or add new content easily.*

## *UI / UX Architecture*

-Animated UI components

-Dynamic layout scaling

-Camera-based cinematic menus

### *Code Quality Notes*

*Urban Entrepreneur maintains improvements from Clay Soldiers while continuing to push scope and complexity. Although a few “god scripts” remain, the architecture is significantly more disciplined and modular.*

# **🗂️ Key Scripts to Review**

## *Core*

*TickSystem.cs*

## *Systems*

*MyGrid.cs*

*MapController.cs*

*Controller.cs*

*OrderManager.cs*

*CalanderController.cs*

*Competitor.cs*

*Employee2.cs*

## *AI*

*C_FindShelf.cs*

*C_GenerateShoppingList.cs*

*E_SetTarget.cs*

*E_Working.cs*

*E_Moving.cs*

*E_FindStockPile.cs*

*E_AtDestination.cs*

*E_FindItem.cs*

*C_ClaimItem.cs*

## *UI*

*ListExtensions.cs*

*Sorter.cs*

*SpecializedSorter.cs*

*UICharacter.cs*

*UITabController.cs*

*UIController.cs*

*ChatMessage.cs*

## *Modding Support*

*Names.cs*

*PersonVisualCon.cs*

*SaveController.cs*

*StartController.cs*

## *Tools*

*RectTransformCopyPaste.cs*

*AutoLocalizer.cs*

*GridOrganizer.cs*

*HeatMap.cs*

*StockZone.cs*


# **🧪 Development Notes**

## **Competition & Public Testing**

-Won 1st Place in the University of Tulsa’s 2023 Global Game Jam professional category

-Extremely high player retention at booth events

-Easily understood after a brief tutorial

-Strong engagement despite competitors’ games nearby

## *Performance Considerations*

-Tick-based update system minimizes heavy Update calls

-Most simulation operations occur in batches

-Modular components allow selective updates

## *Modding*

-Fully moddable via external JSON

-Supports community-driven expansions

-Allows players to adjust values, add new items, or alter maps

## *Behavior & Interaction*

-Employee stress system

-Customer needs & purchase logic

-Store amenities affect customer flow

-Seasonal behavior affects item demand

# **🚧 Why This Project Matters**

## Urban Entrepreneur is a milestone project that demonstrates:

-My ability to create large, interconnected simulation systems

-My maturity in game architecture and data modeling

-The successful launch of a complex game on Steam

-Strong AI logic design using multi-layered decision systems

### The beginning of my consistent engineering style:

-clean architecture

-custom tooling

-performance-first

-modding-aware design

-My capability to produce professional-level titles solo

-Long-term vision executed across multiple systems, maps, and agents

-It is one of the clearest examples of my growth toward senior-level Unity development.

# **📚 Lessons Learned**

-Even well-designed tick systems require load-balancing at scale

-Modding support introduces long-term flexibility and community engagement

-Players benefit greatly from well-structured tutorials

-Seasonal economies and behavioral systems create deep replayability

-God scripts must eventually be replaced by modular architecture

-AI memory systems significantly improve immersion

# **🛠️ Tech Stack**

Unity 2022.3

C#

ScriptableObjects for data

JSON (save/load, modding)

Custom A* pathfinding

Tick-based simulation system

Steamworks.NET

GIMP (2D art)

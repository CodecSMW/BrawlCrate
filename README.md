BrawlCrateLib / BrawlCrate
==========
This fork is focused around handling underdocumented formats.

Current target: MDef

Warning: Most MDef files will not properly save yet. Do not rely on this fork directly unless you are assesesing progress! When ready, features will be provided to the main BrawlCrate repository.

Current status:

Features PSAC does not have:

  Ability to name subroutines
  
  Nameable subroutines
  
  Fighter.pac support
  
  Ability to sort commands by type.
  
  Ability to add crawling, multi-jumps and more to fighters.
  
  Better requirement organization.
  
  An open source codeset when it should have one.

Fighter.Pac: 
  100% Readadable (including Screen Tint and Flash Overlay support as well as external subroutines)
  
  100% Compiling (Warning: PM codes exist that directly inject into those files. They need to ALL be removed or ported simultaneously to have a changed pac file work and the heap sized accordingly!)
  
  Supports renaming actions and subroutines.

Fighter Pac Files:

  Most Readable.
  
  Only several compiling. Extra parameters and articles need case-by-case solutions. R.O.B. functional.
  
  Ability to crawl, glide, tether (with the right module) and have multiple air jumps supported.

Item Pac Files:

  100% Readable
  
  100% Compiling (Still needs testing to verify new features)
  
  Ability to add custom hurtboxes and reflect/shield/absorbers to items present.
  
  Ability to add an item parameter with information on item behavior present.
  
Enemy Pac Files:

  Needing Case-by-Case solutions due to unknown parameters.
  
  All bosses readable.
  
  None compiling yet.


[![GitHub release](https://img.shields.io/github/release/soopercool101/BrawlCrate.svg)](https://github.com/soopercool101/BrawlCrate/releases/latest)
[![GitHub downloads](https://img.shields.io/github/downloads/soopercool101/brawlcrate/total.svg)](https://github.com/soopercool101/BrawlCrate/releases)
[![GitHub latest downloads](https://img.shields.io/github/downloads/soopercool101/brawlcrate/latest/total)](https://github.com/soopercool101/BrawlCrate/releases/latest)
[![Appveyor build status](https://ci.appveyor.com/api/projects/status/github/soopercool101/BrawlCrate?branch=master&svg=true)](https://ci.appveyor.com/project/soopercool101/BrawlCrate)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/e7a263be2c174b0390ae413455bbfcc5)](https://www.codacy.com/app/soopercoolstages/BrawlCrate?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=soopercool101/BrawlCrate&amp;utm_campaign=Badge_Grade)
[![Discord](https://img.shields.io/discord/299006136512806912.svg?logo=discord)](https://discord.gg/s7c8763)

BrawlCrate is a BrawlBox fork with a variety of features aiming to improve ease of modding for Super Smash Bros. Brawl.

BrawlBox and BrawlLib were maintained initially by [Kryal](https://code.google.com/p/brawltools/), then by [BlackJax96](https://code.google.com/p/brawltools2/), and currently by [libertyernie](https://github.com/libertyernie/brawltools).

BrawlCrate also incorporates a modified version of the [Color Smash tool originally by Peter Hatch](https://github.com/PeterHatch/color-smash).

Current BrawlCrate contributors: [soopercool101](https://github.com/soopercool101) and [Squidgy](https://github.com/squidgy617)

Join the [Brawl Knowledge Compendium Discord](https://discord.gg/s7c8763) to talk to the developers.

Older versions can be found on the [BrawlCrate-deprecated repository](https://github.com/soopercool101/BrawlCrate-deprecated).

Documentation for the latest version to be used with the API can be found [here](https://soopercool101.github.io/BrawlCrate/).

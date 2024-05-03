# Virtual Reality Data Visualisation

![](Assets/Resources/Media/plane.jpg)

## How to set up the project

- Sideload the `oxam-vr-demo.apk` file onto your Meta Quest 2, 3 or Pro headset.

- Compile the program yourself
     1) Clone the repository
     2) Follow [this](https://developer.oculus.com/documentation/unity/unity-env-device-setup/) guide to set up a Unity VR development environment 
     3) Build and run the unity project onto the setset via the Andriod Debug Bridge (ADB)

## Supported Datatypes

Data is kept under the `Resources/Data` subfolder, with each csv file stored within a subfolder (a convention that is not strictly neccesary).
There is a `_data.csv` file that lists each current dataset with a user friendly name and the path of the file relative to the `Resources` folder.
To insert your own dataset, store the appropriate CSV file somewhere in the `Resources` folder, then update `_data.csv` with the name and relative path of the new data.

- Raw: A CSV file of format `x, y, z, label` or `x, y, z`
- Time Value: A CSV file of format `time, value` (Ideal for stock data or temperature data over a number of years)
- Orderbooks: A CSV file of format `Quote, EventSymbol, EventTime, BidTime, BidExchangeCode, BidPrice, BidSize, AskTime, AskExchangeCode, AskPrice, AskSize`

## Supported Graph types

At the minute just 2 types of graph are available, but in the future we hope to support more via the `graphRenderer` interface.

- Scatter Plot: A 3D scatter plot  
- Mesh: A 3D plane constructed from a series of 3D points in space

## Supported Colors

- Inferno, Magma, Viridis, Plasma: 4 colourblind-safe colour palletes that are ideal for 3D meshes 
- Static: The whole graph is plotted in a single colour, currently `Color.Red`
- ByLabel: Each label is given a unique color first from 16 predefined and distict colours, then after that a random colour. This happens on a global scale, such that no 2 graphs using this colour scheme will share colours.

## Example Datasets

Datasets are currently kept in the `Assets/Resources/Data` folder. Current datasets included are:

- `Data/AmazonOrderbook`, contains Amazon limit order book data from [LobsterData](https://lobsterdata.com/).
- `Data/AppleOrderbook`, contains Apple limit order book data
- `Data/RadcliffeTemperature`, contains the temperature of the Radcliffe Observatory the dates 1990-01-01 and 2023-12-31.
- `Data/RoyalMailSharePrice`, contains the share price of Royal Mail from [Yahoo Finance](https://uk.finance.yahoo.com/quote/ROYMY/history/?guccounter=1), between the dates 2022-01-01 and 2023-01-01.
- `Data/MusicGenrePCA`, contains music genre feature data from the [GTZAN](https://www.kaggle.com/datasets/andradaolteanu/gtzan-dataset-music-genre-classification/data) dataset reduced into 2D and 3D using PCA.

## Typical Usage

This program is designed to help human pattern recognition of higher dimensional data. It's intended use is to load in 1 or more datasets to be visualised and interacted with, while allowing the dat to be moved, rescaled and compared. The goal of this project is to show that VR data visualisation offers benefits over a traditional 2D projection of the same data. The included datasets are designed to highlight this and the project as a whole has been designed to be extensible and easy to follow - so that it can be built upon and improved over time.



# Program Structure

What follows is a brief overview of the project, as well as some insight on why we chose to make certain design decisons. It will only cover the core components of the program, as the additional scripts should either be irrelevant or trivial to follow.

## Graph.cs

Graph.cs is the backbone of the project, as it defines exactly what data is needed to render a graph, as well as all the getters and setters for said data. It's optimised to do the least amount of processing possible per update and hence many of it's private methods work via side-effects. As will become apparent, the Graph object is passed arounf to the other core components of the program in order to efficiently render each graoh.

## graphRenderer interface

The `graphRenderer` interface defines just a single method: `update`. That method takes in a `Graph` object and then should extract the relevant data to show a graph. This approach is taken because each type of graph needs access to different data and so it is impractical to define an `update` function that takes in a set of predefined arguments. This way, a `graphRenderer` has access to any of the getters in the Graph interface and can safely get any data required.

Note: `ScatterPlot.cs`, `MeshGenerator.cs` and `Axes.cs` all implement this interface 

## dataProcessors

This originally used to be an interface, however the data returned varied from datatype to datatype and so it was impractical to keep the interface. Each `dataProcessor` has the job of taking a raw input from `CSVReader.cs` and processing the data into a list of vectors for the graph interface. Other data may be returned as well, like labels, triangulation data (see `MeshGenerator.cs`) etc.

## Settings.cs

The `Settings.cs` file is a collection of functions that directly hook into the UI that update the appropriate `Graph` object. It keeps a state that includes the current `Graph` object being manipulated, as well as a position vector and a scale vector, again used for updating the `Graph` interface.

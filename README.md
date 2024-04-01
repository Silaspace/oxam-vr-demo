# oxam-vr-demo

## Datasets

Datasets are currently kept in the `Assets/Data` folder. To avoid storing large amounts of data in the Git repository, they kept out of version control by the `.gitignore` file. To download them locally, visit the `Assets/Data` folder and run:

```sh
./_fetchall.sh
```

in the terminal. This will call the `fetch.sh` file in each subfolder, which deals with downloading the relevant data. You can also run

```sh
./_cleanall.sh
```

to remove all downloaded data.

Current datasets included are:

- `Data/AmazonOrderbook`, contains Amazon limit order book data from [LobsterData](https://lobsterdata.com/).
- `Data/MusicGenrePCA`, contains music genre feature data from the [GTZAN](https://www.kaggle.com/datasets/andradaolteanu/gtzan-dataset-music-genre-classification/data) dataset reduced into 2D and 3D using PCA.
- `Data/RoyalMailSharePrice`, contains the share price of Royal Mail from [Yahoo Finance](https://uk.finance.yahoo.com/quote/ROYMY/history/?guccounter=1), between the dates 2022-01-01 and 2023-01-01.

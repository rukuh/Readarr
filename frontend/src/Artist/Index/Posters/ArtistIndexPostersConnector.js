import { createSelector } from 'reselect';
import connectSection from 'Store/connectSection';
import createClientSideCollectionSelector from 'Store/Selectors/createClientSideCollectionSelector';
import createUISettingsSelector from 'Store/Selectors/createUISettingsSelector';
import createDimensionsSelector from 'Store/Selectors/createDimensionsSelector';
import ArtistIndexPosters from './ArtistIndexPosters';

function createMapStateToProps() {
  return createSelector(
    (state) => state.seriesIndex.posterOptions,
    createClientSideCollectionSelector(),
    createUISettingsSelector(),
    createDimensionsSelector(),
    (posterOptions, series, uiSettings, dimensions) => {
      return {
        posterOptions,
        showRelativeDates: uiSettings.showRelativeDates,
        shortDateFormat: uiSettings.shortDateFormat,
        timeFormat: uiSettings.timeFormat,
        isSmallScreen: dimensions.isSmallScreen,
        ...series
      };
    }
  );
}

export default connectSection(
                createMapStateToProps,
                undefined,
                undefined,
                { withRef: true },
                { section: 'series', uiSection: 'seriesIndex' }
              )(ArtistIndexPosters);

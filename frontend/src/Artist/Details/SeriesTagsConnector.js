import _ from 'lodash';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import createArtistSelector from 'Store/Selectors/createArtistSelector';
import createTagsSelector from 'Store/Selectors/createTagsSelector';
import SeriesTags from './SeriesTags';

function createMapStateToProps() {
  return createSelector(
    createArtistSelector(),
    createTagsSelector(),
    (series, tagList) => {
      const tags = _.reduce(series.tags, (acc, tag) => {
        const matchingTag = _.find(tagList, { id: tag });

        if (matchingTag) {
          acc.push(matchingTag.label);
        }

        return acc;
      }, []);

      return {
        tags
      };
    }
  );
}

export default connect(createMapStateToProps)(SeriesTags);

import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import Switch from 'Components/Router/Switch';
import ImportSeriesSelectFolderConnector from 'AddArtist/ImportSeries/SelectFolder/ImportSeriesSelectFolderConnector';
import ImportSeriesConnector from 'AddArtist/ImportSeries/Import/ImportSeriesConnector';

class ImportSeries extends Component {

  //
  // Render

  render() {
    return (
      <Switch>
        <Route
          exact={true}
          path="/add/import"
          component={ImportSeriesSelectFolderConnector}
        />

        <Route
          path="/add/import/:rootFolderId"
          component={ImportSeriesConnector}
        />
      </Switch>
    );
  }
}

export default ImportSeries;

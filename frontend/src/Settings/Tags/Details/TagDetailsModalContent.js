import PropTypes from 'prop-types';
import React from 'react';
import FieldSet from 'Components/FieldSet';
import Label from 'Components/Label';
import Button from 'Components/Link/Button';
import ModalBody from 'Components/Modal/ModalBody';
import ModalContent from 'Components/Modal/ModalContent';
import ModalFooter from 'Components/Modal/ModalFooter';
import ModalHeader from 'Components/Modal/ModalHeader';
import { kinds } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import TagDetailsDelayProfile from './TagDetailsDelayProfile';
import styles from './TagDetailsModalContent.css';

function TagDetailsModalContent(props) {
  const {
    label,
    isTagUsed,
    author,
    delayProfiles,
    importLists,
    notifications,
    releaseProfiles,
    indexers,
    onModalClose,
    onDeleteTagPress
  } = props;

  return (
    <ModalContent onModalClose={onModalClose}>
      <ModalHeader>
        Tag Details - {label}
      </ModalHeader>

      <ModalBody>
        {
          !isTagUsed &&
            <div>
              {translate('TagIsNotUsedAndCanBeDeleted')}
            </div>
        }

        {
          author.length ?
            <FieldSet legend={translate('Authors')}>
              {
                author.map((item) => {
                  return (
                    <div key={item.id}>
                      {item.authorName}
                    </div>
                  );
                })
              }
            </FieldSet> :
            null
        }

        {
          delayProfiles.length ?
            <FieldSet legend={translate('DelayProfile')}>
              {
                delayProfiles.map((item) => {
                  const {
                    id,
                    preferredProtocol,
                    enableUsenet,
                    enableTorrent,
                    usenetDelay,
                    torrentDelay
                  } = item;

                  return (
                    <TagDetailsDelayProfile
                      key={id}
                      preferredProtocol={preferredProtocol}
                      enableUsenet={enableUsenet}
                      enableTorrent={enableTorrent}
                      usenetDelay={usenetDelay}
                      torrentDelay={torrentDelay}
                    />
                  );
                })
              }
            </FieldSet> :
            null
        }

        {
          notifications.length ?
            <FieldSet legend={translate('Connections')}>
              {
                notifications.map((item) => {
                  return (
                    <div key={item.id}>
                      {item.name}
                    </div>
                  );
                })
              }
            </FieldSet> :
            null
        }

        {
          importLists.length ?
            <FieldSet legend={translate('ImportLists')}>
              {
                importLists.map((item) => {
                  return (
                    <div key={item.id}>
                      {item.name}
                    </div>
                  );
                })
              }
            </FieldSet> :
            null
        }

        {
          releaseProfiles.length ?
            <FieldSet legend={translate('ReleaseProfiles')}>
              {
                releaseProfiles.map((item) => {
                  return (
                    <div
                      key={item.id}
                      className={styles.restriction}
                    >
                      <div>
                        {
                          item.required.map((r) => {
                            return (
                              <Label
                                key={r}
                                kind={kinds.SUCCESS}
                              >
                                {r}
                              </Label>
                            );
                          })
                        }
                      </div>

                      <div>
                        {
                          item.ignored.map((i) => {
                            return (
                              <Label
                                key={i}
                                kind={kinds.DANGER}
                              >
                                {i}
                              </Label>
                            );
                          })
                        }s
                      </div>
                    </div>
                  );
                })
              }
            </FieldSet> :
            null
        }

        {
          indexers.length ?
            <FieldSet legend={translate('Indexers')}>
              {
                indexers.map((item) => {
                  return (
                    <div key={item.id}>
                      {item.name}
                    </div>
                  );
                })
              }
            </FieldSet> :
            null
        }
      </ModalBody>

      <ModalFooter>
        {
          <Button
            className={styles.deleteButton}
            kind={kinds.DANGER}
            title={isTagUsed ? translate('IsTagUsedCannotBeDeletedWhileInUse') : undefined}
            isDisabled={isTagUsed}
            onPress={onDeleteTagPress}
          >
            Delete
          </Button>
        }

        <Button
          onPress={onModalClose}
        >
          Close
        </Button>
      </ModalFooter>
    </ModalContent>
  );
}

TagDetailsModalContent.propTypes = {
  label: PropTypes.string.isRequired,
  isTagUsed: PropTypes.bool.isRequired,
  author: PropTypes.arrayOf(PropTypes.object).isRequired,
  delayProfiles: PropTypes.arrayOf(PropTypes.object).isRequired,
  importLists: PropTypes.arrayOf(PropTypes.object).isRequired,
  notifications: PropTypes.arrayOf(PropTypes.object).isRequired,
  releaseProfiles: PropTypes.arrayOf(PropTypes.object).isRequired,
  indexers: PropTypes.arrayOf(PropTypes.object).isRequired,
  onModalClose: PropTypes.func.isRequired,
  onDeleteTagPress: PropTypes.func.isRequired
};

export default TagDetailsModalContent;

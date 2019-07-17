#!/bin/sh

echo $TRAVIS_BRANCH
echo $TRAVIS_COMMIT
echo $TRAVIS_JOB_ID
echo $REPO_COMMIT_AUTHOR
echo $REPO_COMMIT_AUTHOR_EMAIL
echo $REPO_COMMIT_MESSAGE

if [ "$TRAVIS_BRANCH" =  "master" ]
then
    docker-compose -f docker-compose-testing.yml build \
          --build-arg TRAVIS_COMMIT=$TRAVIS_COMMIT \
          --build-arg TRAVIS_BRANCH=$TRAVIS_BRANCH \
          --build-arg TRAVIS_JOB_ID=$TRAVIS_JOB_ID \
          --build-arg REPO_COMMIT_AUTHOR=$REPO_COMMIT_AUTHOR \
          --build-arg REPO_COMMIT_AUTHOR_EMAIL=$REPO_COMMIT_AUTHOR_EMAIL \
          --build-arg REPO_COMMIT_MESSAGE="$REPO_COMMIT_MESSAGE" \
          api.sut

    docker-compose -f docker-compose-testing.yml up --exit-code-from api.sut
else

    docker build \
          -f tests/tests-unit/Dockerfile \
          --build-arg TRAVIS_COMMIT=$TRAVIS_COMMIT \
          --build-arg TRAVIS_BRANCH=$TRAVIS_BRANCH \
          --build-arg TRAVIS_JOB_ID=$TRAVIS_JOB_ID \
          --build-arg REPO_COMMIT_AUTHOR=$REPO_COMMIT_AUTHOR \
          --build-arg REPO_COMMIT_AUTHOR_EMAIL=$REPO_COMMIT_AUTHOR_EMAIL \
          --build-arg REPO_COMMIT_MESSAGE="$REPO_COMMIT_MESSAGE" \
          .
fi
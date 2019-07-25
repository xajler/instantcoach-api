#!/bin/sh

echo $TRAVIS_BRANCH
echo $TRAVIS_COMMIT
echo $TRAVIS_JOB_ID
echo $REPO_COMMIT_AUTHOR
echo $REPO_COMMIT_AUTHOR_EMAIL
echo $REPO_COMMIT_MESSAGE

if [ "$TRAVIS_BRANCH" =  "master" ]
then
    docker-compose -f docker/ci/docker-compose.yml build \
          --build-arg TRAVIS_COMMIT=$TRAVIS_COMMIT \
          --build-arg TRAVIS_BRANCH=$TRAVIS_BRANCH \
          --build-arg TRAVIS_JOB_ID=$TRAVIS_JOB_ID \
          --build-arg REPO_COMMIT_AUTHOR=$REPO_COMMIT_AUTHOR \
          --build-arg REPO_COMMIT_AUTHOR_EMAIL=$REPO_COMMIT_AUTHOR_EMAIL \
          --build-arg REPO_COMMIT_MESSAGE="$REPO_COMMIT_MESSAGE" \
          api.ci

    docker-compose -f docker/ci/docker-compose.yml up --exit-code-from api.ci
else

    docker build \
          -f docker/ci-unit/Dockerfile \
          -t ci_unit:latest \
          --build-arg TRAVIS_COMMIT=$TRAVIS_COMMIT \
          --build-arg TRAVIS_BRANCH=$TRAVIS_BRANCH \
          --build-arg TRAVIS_JOB_ID=$TRAVIS_JOB_ID \
          --build-arg REPO_COMMIT_AUTHOR=$REPO_COMMIT_AUTHOR \
          --build-arg REPO_COMMIT_AUTHOR_EMAIL=$REPO_COMMIT_AUTHOR_EMAIL \
          --build-arg REPO_COMMIT_MESSAGE="$REPO_COMMIT_MESSAGE" \
          .
fi
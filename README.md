# hacker-news-restapi

## Description

This is a little wrapper over https://github.com/HackerNews/API best stories API, which allows to get a provided number of best stories

The service uses Redis cache for caching HackerNews API response in order to reduce load on the API

In addition, this allows to have multiple instances of ***hacker-news-restapi*** services sharing the same cache

## Default configration

Items are cached for 4 hours since they retrieved from the original HackerNews API

This service also assumes that HackerNews API may return not only story ids in BestStories endpoint result

Also some soties could be marked deleted or dead. Those stories are not returned to a caller of ***hacker-news-restapi***

## Prerequisites
<ul>
  <li>K8s installed
    <ul>
      <li>In case of running on Windows - WSL installed</li>
      <li>Was using Rancher Desktop for developmnet env</li>
    </ul>
  </li>
  <li>Helm installed</li>
  <li>
      Helm repo added<br/>
      <code>helm repo add bitnami https://charts.bitnami.com/bitnami</code>
  </li>
  <li>Redis installed<br/>
  <code>helm install redis oci://registry-1.docker.io/bitnamicharts/redis</code>
  </li>
  <li>Net8 SDK</li>
</ul>

## What to improve
<ul>
    <li>Some code refactoring and re-arranging to improve readbility and maintanability</li>
    <li>Unit-tests and E2E tests</li>
    <li>Better logging</li>
    <li>Better exception handling</li>
    <li>Improving the algorythm of refreshing the cache</li>
</ul>
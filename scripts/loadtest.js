import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
  vus: 500, // Virtual users
  duration: '3000s', // Test duration
};

export default function () {
  let wres = http.get('http://dotnet-app:5000/weatherforecast');
  let putres = http.put('http://dotnet-app:5000/person/Smith/John');
  let getres = http.get('http://dotnet-app:5000/person/Smith/John')
  sleep(1);
}
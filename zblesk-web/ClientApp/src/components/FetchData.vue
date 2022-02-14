<template>
<div class="columns">
  <div class="column sassy">
      <small>Component-scoped sass!</small>
  </div>
  <div class="column">
        <h1 id="tableLabel">Weather forecast</h1>
        <p>This component demonstrates fetching data from the server.</p>
        <p v-if="!forecasts"><em>Loading...</em></p>
        <table aria-labelledby="tableLabel" v-if="forecasts">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="forecast of forecasts" v-bind:key="forecast">
                    <td>{{ forecast.date }}</td>
                    <td>{{ forecast.temperatureC }}</td>
                    <td>{{ forecast.temperatureF }}</td>
                    <td>{{ forecast.summary }}</td>
                </tr>
            </tbody>
        </table>
    </div>
  <div class="column"></div>
</div>
</template>

<script>
    import axios from 'axios'
    export default {
        name: "FetchData",
        data() {
            return {
                forecasts: []
            }
        },
        methods: {
            getWeatherForecasts() {
                 axios.get('/weatherforecast')
                     .then((response) => {
                        this.forecasts =  response.data;
                    })
                    .catch(function (error) {
                        alert(error);
                     });
            }
        },
        mounted() {
            this.getWeatherForecasts();
        }
    }
</script>

<!--Scoped Sass-->
<style scoped lang="scss">
table {
    color: #000000;

    tr:nth-child(odd) {
        background-color: #73e6ff;
    }

    tr:nth-child(even) {
        background-color: #9bedff;
    }
}
.sassy {
    color: lightgray;
    text-align: left;
}
</style>
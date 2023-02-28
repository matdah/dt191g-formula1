const app = Vue.createApp({
    data() {
        return {
            drivers: []
        }
    },
    async created() {
        const response = await fetch('http://localhost:5041/api/drivers');
        this.drivers = await response.json();
        console.table(this.drivers);
    }
}).mount("#app");
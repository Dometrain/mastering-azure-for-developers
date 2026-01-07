// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

if (document.getElementById('searchDestinationsButton')) {
    document.getElementById('searchDestinationsButton').addEventListener('click', function () {
        fetch('/destinations/searchDestinations?searchFor=' + document.getElementById('searchFor').value)
            .then(response => response.json())
            .then(data => { 
                let resultDiv = document.getElementById('apiResult');
                resultDiv.innerHTML = '<ul>';
                data.forEach(destination => {
                    resultDiv.innerHTML += `<li>${destination.id} - ${destination.name}</li>`;
                });
                resultDiv.innerHTML += '</ul>';
            })
            .catch(error => console.error('Error:', error));
    });
} 

if (document.getElementById('searchItinerariesButton')) {

    document.getElementById('searchItinerariesButton').addEventListener('click', function () {
        fetch('/itineraries/searchitineraries?searchFor=' + document.getElementById('searchFor').value)
            .then(response => response.json())
            .then(data => {
                let resultDiv = document.getElementById('apiResult');
                resultDiv.innerHTML = '<ul>';
                data.forEach(itinerary => {
                    resultDiv.innerHTML += `<li>${itinerary.id} - ${itinerary.name} - ${itinerary.description} - ${itinerary.userId}</li>`;
                });
                resultDiv.innerHTML += '</ul>';
            })
            .catch(error => console.error('Error:', error));
    });
}
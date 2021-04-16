// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('.js-update-customer').on('click',
    (event) => {
        debugger;
        let firstName = $('.js-first-name').val();
        let lastName = $('.js-last-name').val();
        let customerId = $('.js-customer-id').val();

        console.log(`${firstName} ${lastName}`);

        let data = JSON.stringify({
            firstName: firstName,
            lastName: lastName
        });

        // ajax call
        let result = $.ajax({
            url: `/customer/${customerId}`,
            method: 'PUT',
            contentType: 'application/json',
            data: data
        }).done(response => {
            console.log('Update was successful');
            // success
        }).fail(failure => {
            // fail
            console.log('Update failed');
        });
    });

$('.js-customers-list tbody tr').on('click',
    (event) => {
        console.log($(event.currentTarget).attr('id'));
    });



$('#checkoutButton').on('click',
    (event) => {
        //debugger;

        let cardNumber = $('#cc-number').val();
        let expYear = $('#cc-expirationYear').val();
        let expMonth = $('#cc-expirationMonth').val();
        let amount = $('#cc-amount').val();

        let data = JSON.stringify({
            cardNumber: cardNumber,
            expirationMonth: expMonth,
            expirationYear: expYear,
            amount: amount
        });

        // ajax call

        let result = $.ajax({
            url: `/card/checkout`,
            method: 'POST',
            contentType: 'application/json',
            data: data
        }).done(response => {
            console.log(response);

            console.log('backend call was successful');
            if (response.code == 200) {
                console.log('purchase was successful');
                $('#cardCheckoutStatus').text("purchase was successful!");

            }
            else {
                $('#cardCheckoutStatus').text(response.errorText);
            }
        }).fail(failure => {
            // fail
            console.log('purchase failed');
            $('#cardCheckoutStatus').text("an error has occured!");
        });
    }
);


import 'package:flutter/material.dart';

class ErrorMessage extends StatelessWidget {
  final String message;
  final Function closeError;

  const ErrorMessage({super.key, required this.message, required this.closeError});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(5),
      height: 50,
      width: double.infinity,
      decoration: BoxDecoration(
        color: Colors.red.shade400,
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Expanded(
            child: Text(message, textAlign: TextAlign.center),
          ),
          Center(
            child: IconButton(
              alignment: AlignmentDirectional.topEnd,
              color: Colors.white,
              icon: const Icon(Icons.close),
              onPressed: () {
                closeError.call();
              },
            ),
          )
        ],
      ),
    );
  }
}
